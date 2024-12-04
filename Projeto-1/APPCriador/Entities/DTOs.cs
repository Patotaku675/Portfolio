using System;
using System.Collections.Generic;

namespace APPCriador.Entities
{

    /// <summary>
    /// Criador para ajusar erros de resposta da API
    /// </summary>

    public class CriadorResponseDto
    {
        public int? CriadorId { get; set; }
        public int? UsuarioId { get; set; }
        public List<ConteudoDTO> Conteudos { get; set; }

        public int CurtidasCount { get; set; } // Novo campo para contar as curtidas
    }

    /// <summary>
    /// Representa um criador com suas propriedades e listas de conteúdos e playlists.
    /// </summary>
    public class CriadorDTO
    {
        public int Id { get; set; } // Identificador único do criador
        public string? Nome { get; set; } // Nome do criador
        public List<ConteudoDTO> Conteudos { get; set; } = new List<ConteudoDTO>(); // Lista de conteúdos do criador
        public List<PlaylistDTO> Playlists { get; set; } = new List<PlaylistDTO>(); // Lista de playlists do criador
    }

    /// <summary>
    /// Representa um conteúdo com suas propriedades básicas.
    /// </summary>
    public class ConteudoDTO
    {
        public int Id { get; set; } // Identificador único do conteúdo
        public string? Titulo { get; set; } // Título do conteúdo
        public string? Link { get; set; } // Link associado ao conteúdo
        public int CriadorId { get; set; }
        public int Curtidas { get; set; }  // Aqui é apenas uma contagem de curtidas
    }


    /// <summary>
    /// Representa uma playlist com informações adicionais, incluindo seus conteúdos.
    /// </summary>
    public class PlaylistDTO
    {
        public int Id { get; set; } // Identificador único da playlist
        public string? Nome { get; set; } // Nome da playlist
        public int? CriadorId { get; set; }  // Pode ser nulo se for um usuário
        public int? UsuarioId { get; set; }  // Pode ser nulo se for um criador

        // Lista de conteúdos que pertencem à playlist
        public List<ConteudoDTO> Conteudos { get; set; } = new List<ConteudoDTO>();

        public int Curtidas { get; set; } // Quantidade de curtidas na playlist
        public string? Link { get; set; } // Link relacionado à playlist (ex.: Imagem ou vídeo)
        public string? PrimeiroConteudoLink { get; set; } // Link do primeiro conteúdo da playlist, se existir
    }
}
