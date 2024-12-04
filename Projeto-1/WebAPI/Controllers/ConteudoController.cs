// Controllers/ConteudoController.cs
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Entities;
using WebAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudoController : ControllerBase
    {
        private readonly WebAPIContext _context;

        // Construtor que recebe o contexto da base de dados
        public ConteudoController(WebAPIContext context)
        {
            _context = context;
        }

        // Endpoint para criar um novo conteúdo
        [HttpPost]
        public async Task<IActionResult> CreateConteudo([FromBody] ConteudoInputModel conteudoInput)
        {
            if (conteudoInput == null)
            {
                return BadRequest("Dados de conteúdo inválidos.");
            }

            // Log para visualizar os dados recebidos
            Console.WriteLine($"Título: {conteudoInput.Titulo}, Link: {conteudoInput.Link}, CriadorId: {conteudoInput.CriadorId}");

            // Verifica se o Criador existe no banco de dados
            var criador = await _context.Criadores
                                        .FirstOrDefaultAsync(c => c.Id == conteudoInput.CriadorId);

            if (criador == null)
            {
                return NotFound("Criador não encontrado.");
            }

            // Cria o novo objeto de Conteúdo
            var conteudo = new Conteudo
            {
                Titulo = conteudoInput.Titulo,
                Link = conteudoInput.Link,
                CriadorId = conteudoInput.CriadorId,
                Criador = criador, // Associa o criador ao conteúdo
                Curtidas = new List<Curtida>() // Inicializa a lista de curtidas
            };

            // Adiciona o conteúdo ao banco de dados e salva as alterações
            await _context.Conteudos.AddAsync(conteudo);
            await _context.SaveChangesAsync();

            // Cria o DTO (Data Transfer Object) com os dados do conteúdo
            var conteudoDto = new ConteudoDTO
            {
                Id = conteudo.Id,
                Titulo = conteudo.Titulo,
                Link = conteudo.Link,
                CriadorId = conteudo.CriadorId, // Apenas o CriadorId, não o objeto completo
                Curtidas = conteudo.Curtidas.Count // Conta as curtidas
            };

            // Retorna a resposta de sucesso com o novo conteúdo
            return CreatedAtAction(nameof(GetConteudoById), new { id = conteudo.Id }, conteudoDto);
        }

        // Endpoint para buscar um conteúdo pelo título
        [HttpGet("titulo/{titulo}")]
        public async Task<IActionResult> GetConteudoByTitulo(string titulo)
        {
            // Busca o conteúdo pelo título e inclui as curtidas
            var conteudo = await _context.Conteudos
                .Include(c => c.Curtidas)
                .Where(c => c.Titulo == titulo)
                .Select(c => new ConteudoDTO
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Link = c.Link,
                    CriadorId = c.CriadorId,
                    Curtidas = c.Curtidas.Count // Conta as curtidas do conteúdo
                })
                .FirstOrDefaultAsync();

            if (conteudo == null)
            {
                return NotFound("Conteúdo não encontrado.");
            }

            return Ok(conteudo);
        }

        // Endpoint para listar todos os conteúdos
        [HttpGet]
        public async Task<IActionResult> GetAllConteudos()
        {
            // Busca todos os conteúdos e inclui as curtidas e o nome do criador
            var conteudos = await _context.Conteudos
                .Include(c => c.Curtidas)
                .Select(c => new ConteudoDTO
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Link = c.Link,
                    CriadorId = c.CriadorId,
                    CriadorNome = c.Criador.Nome, // Nome do criador
                    Curtidas = c.Curtidas.Count  // Conta as curtidas do conteúdo
                })
                .ToListAsync();

            return Ok(conteudos);
        }

        // Endpoint para buscar um conteúdo pelo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConteudoById(int id)
        {
            // Busca o conteúdo pelo ID e inclui as curtidas
            var conteudo = await _context.Conteudos
                .Include(c => c.Curtidas)
                .Where(c => c.Id == id)
                .Select(c => new ConteudoDTO
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Link = c.Link,
                    CriadorId = c.CriadorId,
                    Curtidas = c.Curtidas.Count // Conta as curtidas do conteúdo
                })
                .FirstOrDefaultAsync();

            if (conteudo == null)
            {
                return NotFound("Conteúdo não encontrado.");
            }

            return Ok(conteudo);
        }

        // Endpoint para curtir um conteúdo
        [HttpPost("{conteudoId}/curtir")]
        public async Task<IActionResult> CurtirConteudo(int conteudoId)
        {
            // Verifica se o conteúdo existe
            var conteudo = await _context.Conteudos.FindAsync(conteudoId);
            if (conteudo == null)
            {
                return NotFound("Conteúdo não encontrado.");
            }

            // Cria uma nova curtida associada ao conteúdo
            var novaCurtida = new Curtida
            {
                ConteudoId = conteudoId,
                Conteudo = conteudo
            };

            // Adiciona a curtida ao banco de dados e salva as alterações
            _context.Curtidas.Add(novaCurtida);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Curtida registrada com sucesso." });
        }

        // Endpoint para obter o número de curtidas de um conteúdo
        [HttpGet("{conteudoId}/curtidas")]
        public async Task<IActionResult> GetCurtidasConteudo(int conteudoId)
        {
            // Busca o conteúdo e inclui as curtidas
            var conteudo = await _context.Conteudos
                .Include(c => c.Curtidas)
                .FirstOrDefaultAsync(c => c.Id == conteudoId);

            if (conteudo == null)
            {
                return NotFound("Conteúdo não encontrado.");
            }

            // Retorna a quantidade de curtidas
            return Ok(new { Curtidas = conteudo.Curtidas.Count });
        }

        // Endpoint para excluir um conteúdo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConteudo(int id)
        {
            // Encontra o conteúdo pelo ID
            var conteudo = await _context.Conteudos.FindAsync(id);

            if (conteudo == null)
            {
                return NotFound(new { Message = "Conteúdo não encontrado." });
            }

            // Remove o conteúdo do banco de dados
            _context.Conteudos.Remove(conteudo);
            await _context.SaveChangesAsync();

            // Retorna 204 (sem conteúdo) para indicar sucesso
            return NoContent();
        }
    }

    // Modelo de entrada para criação de Conteúdo
    public class ConteudoInputModel
    {
        public required string Titulo { get; set; }
        public required string Link { get; set; }
        public int CriadorId { get; set; }
    }
}
