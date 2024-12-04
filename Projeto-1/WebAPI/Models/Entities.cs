using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entities
{
    // Classe que representa um Usuário
    public class Usuario
    {
        // Identificador único do usuário
        [Key]
        public int Id { get; set; }

        // Nome do usuário (campo obrigatório)
        [Required]
        public required string Nome { get; set; }

        // Email do usuário (campo obrigatório)
        [Required]
        public required string Email { get; set; }
    }

    // Classe que representa um Criador
    public class Criador
    {
        // Identificador único do criador
        [Key]
        public int Id { get; set; }

        // Nome do criador (campo obrigatório)
        [Required]
        public required string Nome { get; set; }

        // Lista de conteúdos criados pelo criador
        public List<Conteudo> Conteudos { get; set; } = new List<Conteudo>();

        // Relacionamento com as playlists criadas pelo criador
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }

    // Classe que representa uma Playlist
    public class Playlist
    {
        // Identificador único da playlist
        public int Id { get; set; }

        // Nome da playlist
        public string? Nome { get; set; }

        // Identificador do criador associado à playlist (opcional)
        public int? CriadorId { get; set; }

        // Relacionamento com o criador
        public Criador? Criador { get; set; }

        // Identificador do usuário associado à playlist (opcional)
        public int? UsuarioId { get; set; }

        // Relacionamento com o usuário
        public Usuario? Usuario { get; set; }

        // Lista de conteúdos associados à playlist
        public List<Conteudo> Conteudos { get; set; } = new List<Conteudo>();
    }

    // Classe que representa um Conteúdo
    public class Conteudo
    {
        // Identificador único do conteúdo
        [Key]
        public int Id { get; set; }

        // Título do conteúdo (campo obrigatório)
        [Required]
        public required string Titulo { get; set; }

        // Link do conteúdo (campo obrigatório)
        [Required]
        public required string Link { get; set; }

        // Identificador do criador do conteúdo
        [ForeignKey("Criador")]
        public int CriadorId { get; set; }

        // Relacionamento com o criador
        public required Criador Criador { get; set; }

        // Relacionamento com as curtidas do conteúdo
        public required ICollection<Curtida> Curtidas { get; set; }

        // Não há chave estrangeira para Playlist, pois o conteúdo não precisa estar associado a uma playlist
    }

    // Classe que representa uma Curtida
    public class Curtida
    {
        // Identificador único da curtida
        [Key]
        public int Id { get; set; }

        // Identificador do conteúdo associado à curtida
        public int ConteudoId { get; set; }

        // Relacionamento com o conteúdo
        public Conteudo? Conteudo { get; set; } // Inicializado automaticamente no controlador
    }

    // Classe que representa o repositório de playlists
    public class PlaylistRepository
    {
        // Lista de playlists armazenadas
        private readonly List<Playlist> _playlists = new();

        // Método para retornar todas as playlists
        public IEnumerable<Playlist> GetAllPlaylists() => _playlists;

        // Método para retornar uma playlist pelo seu ID
        // Retorna um valor nulo caso não encontre a playlist
        public Playlist? GetPlaylistById(int id) =>
            _playlists.Find(p => p.Id == id);

        // Método para adicionar uma nova playlist
        public void AddPlaylist(Playlist playlist) =>
            _playlists.Add(playlist);

        // Método para atualizar uma playlist existente
        public void UpdatePlaylist(Playlist playlist)
        {
            var index = _playlists.FindIndex(p => p.Id == playlist.Id);
            if (index != -1) _playlists[index] = playlist;
        }

        // Método para excluir uma playlist pelo ID
        public void DeletePlaylist(int id) =>
            _playlists.RemoveAll(p => p.Id == id);
    }

    // Classe que representa a requisição para adicionar um item (conteúdo) a uma playlist
    public class AdicionarItemRequest
    {
        // Identificador do conteúdo a ser adicionado
        public int? ConteudoId { get; set; }

        // Identificador da playlist onde o conteúdo será adicionado
        public int? PlaylistId { get; set; }
    }
}
