// Controllers/CriadorController.cs
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Entities;
using WebAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CriadorController : ControllerBase
    {
        private readonly WebAPIContext _context;

        // Construtor que recebe o contexto da base de dados
        public CriadorController(WebAPIContext context)
        {
            _context = context;
        }

        // Endpoint para criar um novo criador
        [HttpPost]
        public async Task<IActionResult> CreateCriador([FromBody] Criador criador)
        {
            // Verifica se o criador é nulo
            if (criador == null)
            {
                return BadRequest("Criador inválido.");
            }

            // Adiciona o criador ao contexto e salva no banco de dados
            _context.Criadores.Add(criador);
            await _context.SaveChangesAsync();

            // Retorna o criador criado com a resposta de sucesso
            return CreatedAtAction(nameof(GetCriadorById), new { id = criador.Id }, criador);
        }

        // Endpoint para buscar um criador pelo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCriadorById(int id)
        {
            // Busca o criador pelo ID e inclui os conteúdos e playlists
            var criador = await _context.Criadores
                .Include(c => c.Conteudos)  // Inclui os conteúdos do criador
                .ThenInclude(co => co.Curtidas)  // Inclui as curtidas dos conteúdos
                .Include(c => c.Playlists)  // Inclui as playlists do criador
                .FirstOrDefaultAsync(c => c.Id == id);

            // Verifica se o criador foi encontrado
            if (criador == null)
            {
                return NotFound("Criador não encontrado.");
            }

            // Cria o DTO para o criador, incluindo seus conteúdos e playlists
            var criadorDto = new CriadorDTO
            {
                Id = criador.Id,
                Nome = criador.Nome,
                Conteudos = criador.Conteudos.Select(co => new ConteudoDTO
                {
                    Id = co.Id,
                    Titulo = co.Titulo,
                    Link = co.Link,
                    Curtidas = co.Curtidas?.Count ?? 0
                }).ToList(),
                Playlists = criador.Playlists.Select(p => new PlaylistDto
                {
                    Id = p.Id,
                    Nome = p.Nome
                }).ToList()
            };

            return Ok(criadorDto);
        }

        // Endpoint para deletar um criador
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCriador(int id)
        {
            // Busca o criador pelo ID
            var criador = await _context.Criadores.FindAsync(id);

            // Verifica se o criador foi encontrado
            if (criador == null)
            {
                return NotFound();
            }

            // Remove o criador do banco de dados
            _context.Criadores.Remove(criador);
            await _context.SaveChangesAsync();

            // Retorna 204, indicando que a operação foi bem-sucedida sem conteúdo de retorno
            return NoContent();
        }

        // Endpoint para listar todos os criadores
        [HttpGet]
        public async Task<IActionResult> GetAllCriadores()
        {
            // Busca todos os criadores e inclui seus conteúdos e playlists
            var criadores = await _context.Criadores
                .Include(c => c.Conteudos)  // Inclui os conteúdos do criador
                .ThenInclude(co => co.Curtidas)  // Inclui as curtidas dos conteúdos
                .Include(c => c.Playlists)  // Inclui as playlists do criador
                .ToListAsync();

            // Cria os DTOs para todos os criadores, incluindo conteúdos e playlists
            var criadoresDto = criadores.Select(c => new CriadorDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Conteudos = (c.Conteudos ?? new List<Conteudo>()).Select(co => new ConteudoDTO
                {
                    Id = co.Id,
                    Titulo = co.Titulo,
                    Curtidas = co.Curtidas?.Count ?? 0 
                }).ToList(),
                Playlists = (c.Playlists ?? new List<Playlist>()).Select(p => new PlaylistDto
                {
                    Id = p.Id,
                    Nome = p.Nome
                }).ToList()
            }).ToList();

            return Ok(criadoresDto);
        }



    }
}
