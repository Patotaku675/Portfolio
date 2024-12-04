using Microsoft.Maui.Controls;
using System.Linq;

namespace APPCriador.Pages
{
    public partial class PaginaVisualizacao : ContentPage
    {
        public string VideoTitle { get; set; }

        public PaginaVisualizacao(string url, string videoTitle)
        {
            InitializeComponent();

            Title = "Visualizando o Conteúdo";
            VideoTitle = videoTitle;
            BindingContext = this;

            // Verifica o tipo de conteúdo e exibe no WebView
            if (IsYouTubeUrl(url))
            {
                ShowYouTubeVideo(url);
            }
            else if (IsImageUrl(url))
            {
                ShowImageInWebView(url);
            }
            else
            {
                DisplayAlert("Erro", "O tipo de conteúdo não é suportado.", "OK");
            }
        }

        private bool IsYouTubeUrl(string url)
        {
            return url.Contains("youtu.be") || url.Contains("youtube.com");
        }

        private bool IsImageUrl(string url)
        {
            var path = url.Split('?')[0]; // Remove os parâmetros após a interrogação
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            return imageExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private void ShowYouTubeVideo(string url)
        {
            var embedUrl = ConvertToEmbedUrl(url);
            webView.Source = embedUrl;
        }

        private string ConvertToEmbedUrl(string url)
        {
            var videoId = UrlUtils.ExtractYouTubeVideoId(url);
            return !string.IsNullOrEmpty(videoId)
                ? $"https://www.youtube.com/embed/{videoId}?rel=0&autohide=1&showinfo=0&modestbranding=1&fs=1"
                : url;
        }

        private void ShowImageInWebView(string url)
        {
            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            margin: 0;
                            padding: 0;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                            background-color: #fff;
                        }}
                        img {{
                            max-width: 100%;
                            max-height: 100%;
                        }}
                    </style>
                </head>
                <body>
                    <img src='{url}' alt='Imagem não carregada' />
                </body>
                </html>";

            webView.Source = new HtmlWebViewSource
            {
                Html = htmlContent
            };
        }
    }
}
