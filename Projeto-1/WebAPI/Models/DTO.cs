namespace WebAPI.Models
{
    // Classe que representa os dados de um conteúdo (DTO)
    public class ConteudoDTO
    {
        // Identificador único do conteúdo
        public int Id { get; set; }

        // Identificador do criador do conteúdo
        public int CriadorId { get; set; }

        // Nome do criador do conteúdo
        public string? CriadorNome { get; set; }

        // Título do conteúdo
        public string? Titulo { get; set; }

        // Link para o conteúdo
        public string? Link { get; set; }

        // Número de curtidas que o conteúdo recebeu
        public int Curtidas { get; set; }
    }

    // Classe que representa uma playlist (DTO)
    public class PlaylistDto
    {
        // Identificador único da playlist
        public int Id { get; set; }

        // Nome da playlist
        public string? Nome { get; set; }

        // Identificador do criador da playlist
        public int? CriadorId { get; set; }

        // Identificador do usuário associado à playlist (opcional)
        public int? UsuarioId { get; set; }

        // Link do primeiro conteúdo da playlist
        public string? PrimeiroConteudoLink { get; set; }

        // Lista de conteúdos associados à playlist
        public List<ConteudoDTO> Conteudos { get; set; } = new List<ConteudoDTO>();
    }

    // Classe que representa os dados de um criador (DTO)
    public class CriadorDTO
    {
        // Identificador único do criador
        public int Id { get; set; }

        // Nome do criador
        public string? Nome { get; set; }

        // Lista de conteúdos criados pelo criador
        public List<ConteudoDTO> Conteudos { get; set; } = new List<ConteudoDTO>();

        // Lista de playlists criadas pelo criador
        public List<PlaylistDto> Playlists { get; set; } = new List<PlaylistDto>();
    }
}
