// Controllers/UsuarioController.cs
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Entities;
using WebAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    // Definindo a rota base para os endpoints do controlador
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly WebAPIContext _context;

        // Construtor para injeção de dependência
        public UsuarioController(WebAPIContext context)
        {
            _context = context;
        }

        // Método para criar um novo usuário
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] Usuario usuario)
        {
            // Verifica se o usuário enviado é válido
            if (usuario == null)
            {
                return BadRequest("Usuário inválido.");
            }

            // Adiciona o novo usuário ao banco de dados
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Retorna a resposta com o usuário recém-criado
            return CreatedAtAction(nameof(GetUsuarioById), new { id = usuario.Id }, usuario);
        }

        // Método para obter um usuário por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            // Busca o usuário no banco de dados pelo ID
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Retorna os dados do usuário encontrado
            return Ok(usuario);
        }

        // Método para excluir um usuário pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            // Busca o usuário no banco de dados
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Remove o usuário encontrado
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            // Retorna uma resposta de sucesso (sem conteúdo)
            return NoContent();
        }

        // Método para obter todos os usuários
        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            // Busca todos os usuários no banco de dados
            var usuarios = await _context.Usuarios.ToListAsync();

            // Retorna a lista de usuários encontrados
            return Ok(usuarios);
        }
    }
}
