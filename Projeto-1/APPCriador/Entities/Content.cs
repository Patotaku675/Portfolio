using System;

namespace APPCriador.Entities
{
    /// Representa o modelo de um conteúdo (Content) com informações básicas.
    public class Content
    {

        // Identificador único do conteúdo.
        public int Id { get; set; }

        // Título do conteúdo.
        public string? Title { get; set; }

        // Link associado ao conteúdo (pode ser um vídeo ou imagem).
        public string? Link { get; set; }

        // Quantidade de curtidas do conteúdo.
        public int Likes { get; set; }
    }
}
