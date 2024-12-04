// Controllers/PlaylistsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebAPI.Data;
using WebAPI.Entities;
using WebAPI.Models;

public class PlaylistCreateDto
{
    public int Id { get; set; }

    // Nome da playlist, obrigatório
    [Required]
    public required string Nome { get; set; }

    // ID do criador e do usuário, ambos opcionais
    public int? CriadorId { get; set; }
    public int? UsuarioId { get; set; }
}

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public PlaylistsController(WebAPIContext context)
        {
            _context = context;
        }

        // Método para criar uma nova playlist
        [HttpPost]
        public async Task<ActionResult<PlaylistDto>> CreatePlaylist([FromBody] PlaylistCreateDto playlistCreateDto)
        {
            // Verifica se CriadorId ou UsuarioId foi informado
            if (playlistCreateDto.CriadorId <= 0 && playlistCreateDto.UsuarioId <= 0)
            {
                return BadRequest("CriadorId ou UsuarioId é obrigatório.");
            }

            // Verifica se o Criador ou o Usuario existe no banco de dados
            Criador? criador = null;
            Usuario? usuario = null;

            if (playlistCreateDto.CriadorId.HasValue)
            {
                criador = await _context.Criadores.FirstOrDefaultAsync(c => c.Id == playlistCreateDto.CriadorId.Value);
            }

            if (playlistCreateDto.UsuarioId.HasValue)
            {
                usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == playlistCreateDto.UsuarioId.Value);
            }

            // Se nenhum dos dois for encontrado, retorna erro
            if (criador == null && usuario == null)
            {
                return NotFound("Criador ou Usuário não encontrado.");
            }

            // Cria a playlist com base nos dados informados
            var playlist = new Playlist
            {
                Nome = playlistCreateDto.Nome,
                CriadorId = criador?.Id,
                Criador = criador,
                UsuarioId = usuario?.Id,
                Usuario = usuario
            };

            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();

            var playlistDto = new PlaylistDto
            {
                Id = playlist.Id,
                Nome = playlist.Nome,
                CriadorId = playlist.CriadorId,
                UsuarioId = playlist.UsuarioId
            };

            return CreatedAtAction(nameof(GetPlaylists), new { id = playlist.Id }, playlistDto);
        }


        // Método para adicionar conteúdos a uma playlist via URL
        [HttpPost("{playlistId}/conteudos")]
        public async Task<IActionResult> AddConteudosToPlaylist(int playlistId, [FromQuery] List<int> conteudosIds)
        {
            // Verifica se a playlist existe
            var playlist = await _context.Playlists
                .Include(p => p.Conteudos)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
            {
                return NotFound(new { message = "Playlist não encontrada" });
            }

            // Adiciona os conteúdos à playlist
            foreach (var conteudoId in conteudosIds)
            {
                var conteudo = await _context.Conteudos.FirstOrDefaultAsync(c => c.Id == conteudoId);
                if (conteudo != null && !playlist.Conteudos.Contains(conteudo))
                {
                    playlist.Conteudos.Add(conteudo);
                }
            }

        // Salva as alterações no banco de dados
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok(new { message = "Conteúdos adicionados com sucesso" });
        }
        else
        {
            return StatusCode(500, new { message = "Erro ao tentar adicionar conteúdos à playlist" });
        }
    }



        // Método para deletar conteúdos de uma playlist
        [HttpDelete("{playlistId}/conteudos")]
        public async Task<IActionResult> DeleteConteudosFromPlaylist(int playlistId, [FromQuery] List<int> conteudosIds)
        {
            // Verifica se a playlist existe
            var playlist = await _context.Playlists
                .Include(p => p.Conteudos)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
            {
                return NotFound(new { message = "Playlist não encontrada" });
            }

            // Remove os conteúdos da playlist
            foreach (var conteudoId in conteudosIds)
            {
                var conteudo = playlist.Conteudos.FirstOrDefault(c => c.Id == conteudoId);
                if (conteudo != null)
                {
                    playlist.Conteudos.Remove(conteudo);
                }
            }

            // Salva as alterações no banco de dados
            var result = await _context.SaveChangesAsync();
            
            if (result > 0)
            {
                return Ok(new { message = "Conteúdos removidos com sucesso" });
            }
            else
            {
                return StatusCode(500, new { message = "Erro ao tentar remover conteúdos da playlist" });
            }
        }

        // Método para buscar uma playlist pelo nome
        [HttpGet("nome/{nome}")]
        public async Task<IActionResult> GetPlaylistByName(string nome)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var playlist = await _context.Playlists
                .Include(p => p.Conteudos)
                .Include(p => p.Criador)
                .Include(p => p.Usuario)
                .Where(p => p.Nome.Contains(nome))
                .Select(p => new
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    CriadorId = p.CriadorId,
                    CriadorNome = p.Criador != null ? p.Criador.Nome : null,
                    UsuarioId = p.UsuarioId,
                    UsuarioNome = p.Usuario != null ? p.Usuario.Nome : null,
                    Conteudos = p.Conteudos.Select(conteudo => new
                    {
                        Id = conteudo.Id,
                        Titulo = conteudo.Titulo,
                        Link = conteudo.Link
                    }).ToList()
                })
                .FirstOrDefaultAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (playlist == null)
            {
                return NotFound("Playlist não encontrada.");
            }

            // Obtém o link do primeiro conteúdo da playlist
            var primeiroConteudoLink = playlist.Conteudos.OrderBy(c => c.Id).FirstOrDefault()?.Link;

            // Adiciona o campo PrimeiroConteudoLink à resposta
            var response = new
            {
                playlist.Id,
                playlist.Nome,
                playlist.CriadorId,
                playlist.CriadorNome,
                playlist.UsuarioId,
                playlist.UsuarioNome,
                PrimeiroConteudoLink = primeiroConteudoLink,
                playlist.Conteudos
            };

            return Ok(response);
        }

        // Método para buscar playlists de um criador
        [HttpGet("criador/{criadorId}")]
        public async Task<IActionResult> GetPlaylistsByCriadorId(int criadorId)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var playlists = await _context.Playlists
                .Where(p => p.CriadorId == criadorId)
                .Include(p => p.Conteudos)
                .Select(p => new
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    CriadorId = p.CriadorId,
                    UsuarioId = p.UsuarioId,
                    PrimeiroConteudoLink = p.Conteudos.OrderBy(c => c.Id).FirstOrDefault() != null 
                                        ? p.Conteudos.OrderBy(c => c.Id).FirstOrDefault().Link
                                        : "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png",
                    Conteudos = p.Conteudos.Select(c => new
                    {
                        Id = c.Id,
                        Titulo = c.Titulo,
                        Link = c.Link
                    }).ToList()
                })
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (!playlists.Any())
            {
                return NotFound("Nenhuma playlist encontrada para o criador fornecido.");
            }

            return Ok(playlists);
        }

        // Método para buscar playlists de um usuário
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetPlaylistsByUsuarioId(int usuarioId)
        {
            var playlists = await _context.Playlists
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Conteudos)
                .ThenInclude(c => c.Criador)
                .Include(p => p.Conteudos)
                .ThenInclude(c => c.Curtidas)
                .Select(p => new
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    CriadorId = p.CriadorId.HasValue ? p.CriadorId : (int?)null,
                    UsuarioId = p.UsuarioId.HasValue ? p.UsuarioId : (int?)null,
                    Conteudos = p.Conteudos.Select(c => new
                    {
                        Id = c.Id,
                        Titulo = c.Titulo,
                        CriadorId = c.CriadorId,
                        CriadorNome = c.Criador.Nome,
                        Link = c.Link,
                        Curtidas = c.Curtidas.Count
                    }).ToList()
                })
                .ToListAsync();

            if (!playlists.Any())
            {
                return NotFound("Nenhuma playlist encontrada para o UsuarioId fornecido.");
            }

            return Ok(playlists);
        }

        // Método para buscar uma playlist pelo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistById(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Conteudos)
                .Include(p => p.Criador)
                .Include(p => p.Usuario)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    CriadorId = p.CriadorId,
                    CriadorNome = p.Criador != null ? p.Criador.Nome : null,
                    UsuarioId = p.UsuarioId,
                    UsuarioNome = p.Usuario != null ? p.Usuario.Nome : null,
                    Conteudos = p.Conteudos.Select(conteudo => new
                    {
                        Id = conteudo.Id,
                        Titulo = conteudo.Titulo,
                        Link = conteudo.Link
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (playlist == null)
            {
                return NotFound("Playlist não encontrada.");
            }

            return Ok(playlist);
        }

        // Método para obter todas as playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPlaylists()
        {
            var playlists = await _context.Playlists
                .Include(p => p.Conteudos)
                    .ThenInclude(c => c.Curtidas) // Inclui as curtidas de cada conteúdo
                .Include(p => p.Criador)
                .Include(p => p.Usuario)
                .Select(p => new
                {
                    p.Id,
                    p.Nome,
                    p.CriadorId,
                    CriadorNome = p.Criador != null ? p.Criador.Nome : null,
                    p.UsuarioId,
                    UsuarioNome = p.Usuario != null ? p.Usuario.Nome : null,
                    Conteudos = p.Conteudos.Select(conteudo => new
                    {
                        conteudo.Id,
                        conteudo.Titulo,
                        conteudo.Link,
                        Curtidas = conteudo.Curtidas.Count(), // Contagem de curtidas
                        CriadorNome = conteudo.Criador.Nome
                    }).ToList(), // Não precisa de ToListAsync aqui, pois já está materializando a lista
                    PrimeiroConteudoLink = p.Conteudos.OrderBy(c => c.Id).FirstOrDefault() != null ? p.Conteudos.OrderBy(c => c.Id).FirstOrDefault().Link : null
                })
                .ToListAsync(); // ToListAsync no final da consulta, após a materialização

            return Ok(playlists);
        }

        



    }
}
