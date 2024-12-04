using Microsoft.EntityFrameworkCore;
using WebAPI.Entities;

namespace WebAPI.Data
{
    // Contexto do banco de dados da aplicação, gerenciando as entidades
    public class WebAPIContext : DbContext
    {
        // Construtor que recebe as opções de configuração do DbContext
        public WebAPIContext(DbContextOptions<WebAPIContext> options) : base(options) { }

        // Conjunto de dados para a tabela 'Usuarios' no banco
        public required DbSet<Usuario> Usuarios { get; set; }

        // Conjunto de dados para a tabela 'Criadores' no banco
        public required DbSet<Criador> Criadores { get; set; }

        // Conjunto de dados para a tabela 'Playlists' no banco
        public required DbSet<Playlist> Playlists { get; set; }

        // Conjunto de dados para a tabela 'Conteudos' no banco
        public required DbSet<Conteudo> Conteudos { get; set; }

        // Conjunto de dados para a tabela 'Curtidas' no banco
        public required DbSet<Curtida> Curtidas { get; set; }

        // Método para configurar os relacionamentos e comportamentos entre as entidades
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração do relacionamento entre Playlist e Criador
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.Criador) // Uma Playlist tem um Criador
                .WithMany(c => c.Playlists) // Um Criador pode ter várias Playlists
                .HasForeignKey(p => p.CriadorId); // Chave estrangeira para o Criador

            // Configuração do relacionamento muitos para muitos entre Playlist e Conteudo
            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Conteudos) // Uma Playlist pode ter vários Conteúdos
                .WithMany() // Conteúdo não precisa se referir diretamente à Playlist
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistConteudo", // Nome da tabela de junção
                    j => j.HasOne<Conteudo>().WithMany().HasForeignKey("ConteudoId"), // Chave estrangeira para Conteúdo
                    j => j.HasOne<Playlist>().WithMany().HasForeignKey("PlaylistId"), // Chave estrangeira para Playlist
                    j =>
                    {
                        // Definindo a chave primária composta para a tabela de junção
                        j.HasKey("ConteudoId", "PlaylistId");
                    });

            // Configuração do relacionamento entre Conteudo e Curtida
            modelBuilder.Entity<Curtida>()
                .HasOne(c => c.Conteudo) // Cada Curtida está associada a um Conteúdo
                .WithMany(c => c.Curtidas) // Um Conteúdo pode ter várias Curtidas
                .HasForeignKey(c => c.ConteudoId) // Chave estrangeira para o Conteúdo
                .OnDelete(DeleteBehavior.Cascade); // Quando um Conteúdo for excluído, as Curtidas associadas também serão excluídas

            // Chama a configuração base para aplicar o mapeamento de modelo
            base.OnModelCreating(modelBuilder);
        }
    }
}
