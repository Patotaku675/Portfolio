using APPCriador.Entities;
using APPCriador.Services;
using SkiaSharp;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace APPCriador.Pages
{
    public partial class PaginaDeConteudos : ContentPage
    {
        private ApiService _apiService; // Serviço para interagir com a API
        private int _CriadorId; // ID do criador que está armazenado nas preferências
        private string _CriadorNome; // Nome do criador que está armazenado nas preferências

        // Construtor da página
        public PaginaDeConteudos()
        {
            InitializeComponent(); // Inicializa os componentes da página
            _apiService = new ApiService(); // Inicializa o serviço de API
            _CriadorId = Preferences.Get("CriadorId", -1); // Recupera o CriadorId armazenado nas preferências
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o CriadorNome armazenado nas preferências
            // Define o Texto da página
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // Método para redimensionar a imagem
        private async Task<SKBitmap> RedimensionarImagemAsync(string url, int novaLargura, int novaAltura)
        {
            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(url);  // Obtém os bytes da imagem a partir da URL

                using (var stream = new MemoryStream(imageBytes))
                {
                    // Carrega a imagem no SkiaSharp
                    var originalBitmap = SKBitmap.Decode(stream);

                    // Redimensiona a imagem para a nova resolução
                    var resizedBitmap = originalBitmap.Resize(new SKImageInfo(novaLargura, novaAltura), SKFilterQuality.Medium);

                    return resizedBitmap; // Retorna a imagem redimensionada
                }
            }
        }

        // Método para exibir os conteúdos na tela
        private async void ExibirConteudos(List<ConteudoDTO> conteudos)
        {
            MeuFlexLayout.Children.Clear(); // Limpa o FlexLayout antes de adicionar os novos conteúdos

            foreach (var conteudo in conteudos)
            {
                var grid = new Grid
                {
                    WidthRequest = 200, // Largura do grid
                    HeightRequest = 160, // Altura do grid
                    Margin = new Thickness(5), // Margem do grid
                    BackgroundColor = Colors.Transparent // Fundo transparente
                };

                // Define a URL da imagem de mídia
                string midiaUrl = string.IsNullOrEmpty(conteudo.Link) || !conteudo.Link.StartsWith("https://")
                                  ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padrão
                                  : conteudo.Link;

                // Verifica se a URL é do YouTube
                bool isYouTube = midiaUrl.Contains("youtu.be") || midiaUrl.Contains("youtube.com");

                string imageUrl;
                if (isYouTube)
                {
                    // Se for YouTube, extrai o ID do vídeo e utiliza o thumbnail do vídeo
                    var videoId = UrlUtils.ExtractYouTubeVideoId(midiaUrl);
                    if (string.IsNullOrEmpty(videoId))
                    {
                        // Se o ID do vídeo for inválido, usa uma imagem padrão
                        imageUrl = "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png";
                    }
                    else
                    {
                        // Caso contrário, usa o thumbnail do YouTube
                        imageUrl = $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
                    }
                }
                else
                {
                    // Se não for YouTube, utiliza o link da mídia diretamente
                    imageUrl = midiaUrl;
                }

                // Redimensiona a imagem para exibição
                var redimensionarImagemTask = RedimensionarImagemAsync(imageUrl, 180, 100);
                var resizedBitmap = await redimensionarImagemTask;

                // Codifica a imagem redimensionada para PNG
                var encodedImage = resizedBitmap.Encode(SKEncodedImageFormat.Png, 100);

                // Converte a imagem codificada para um ImageSource
                var imageSource = ImageSource.FromStream(() => encodedImage.AsStream());

                // Cria o elemento de imagem
                var mediaElement = new Image
                {
                    Source = imageSource, // Define a fonte da imagem
                    Aspect = Aspect.AspectFill, // Define o aspecto da imagem
                    WidthRequest = 180, // Largura da imagem
                    HeightRequest = 100, // Altura da imagem
                    Margin = new Thickness(0, 8, 0, 0), // Margem superior para deslocar a imagem
                    BackgroundColor = Colors.Transparent // Fundo transparente
                };

                // Cria o título do conteúdo
                var titleLabel = new Label
                {
                    Text = conteudo.Titulo, // Define o título
                    HorizontalOptions = LayoutOptions.Center, // Centraliza horizontalmente
                    VerticalOptions = LayoutOptions.Start, // Alinha ao topo
                    FontAttributes = FontAttributes.Bold, // Define o título em negrito
                    FontFamily = "Times New Roman", // Define a fonte
                    FontSize = 28, // Define o tamanho da fonte
                    Margin = new Thickness(0, 0, 0, 0), // Margem do título
                    TextColor = Colors.Black, // Cor do texto
                    BackgroundColor = Colors.Transparent, // Fundo transparente
                    LineBreakMode = LineBreakMode.TailTruncation, // Impede a quebra de linha e corta o texto
                    MaxLines = 1 // Limita a exibição a uma linha
                };


                // Cria o label de curtidas
                var curtidasLabel = new Label
                {
                    Text = $"Likes: {conteudo.Curtidas}", // Exibe o número de curtidas
                    HorizontalOptions = LayoutOptions.Center, // Centraliza horizontalmente
                    VerticalOptions = LayoutOptions.End, // Alinha ao final
                    FontFamily = "Times New Roman", // Define a fonte
                    FontSize = 20, // Define o tamanho da fonte
                    Margin = 1, // Margem para o label
                    TextColor = Colors.Black, // Cor do texto
                    BackgroundColor = Colors.Transparent // Fundo transparente
                };

                // Cria o botão para abrir a visualização do conteúdo
                var button = new Button
                {
                    BorderColor = Colors.Transparent, // Remove a borda
                    BorderWidth = 3, // Define a largura da borda
                    CornerRadius = 12, // Define o raio das bordas
                    BackgroundColor = Colors.Transparent // Fundo transparente
                };

                // Evento de clique do botão para abrir a página de visualização
                button.Clicked += async (sender, e) =>
                {
                    // Navega para a página de visualização passando a URL e o título
                    await Navigation.PushAsync(new PaginaVisualizacao(midiaUrl, conteudo.Titulo));
                };

                // Adiciona os elementos ao grid
                grid.Children.Add(mediaElement);
                grid.Children.Add(titleLabel);
                grid.Children.Add(curtidasLabel);
                grid.Children.Add(button);

                // Adiciona o grid ao FlexLayout
                MeuFlexLayout.Children.Add(grid);
            }
        }

        // Método para o botão de adicionar conteúdo
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            // Navega para a página de adicionar novo conteúdo
            await Navigation.PushAsync(new NovoConteudo());
        }

        // Método para o botão de apagar conteúdo
        private async void OnApagarClicked(object sender, EventArgs e)
        {
            // Navega para a página de apagar conteúdo
            await Navigation.PushAsync(new ApagarConteudo());
        }

        // Método chamado quando a página aparece na tela
        protected override async void OnAppearing()
        {
            base.OnAppearing(); // Chama a implementação base

            int criadorId = _CriadorId; // Obtém o ID do criador

            // Busca os dados do criador na API
            var criador = await _apiService.GetCriadorByIdAsync(criadorId);

            // Verifica se o criador existe
            if (criador != null)
            {
                // Verifica se o criador possui conteúdos
                if (criador.Conteudos != null && criador.Conteudos.Count > 0)
                {
                    // Exibe os conteúdos do criador
                    ExibirConteudos(criador.Conteudos);
                }
                else
                {
                    // Caso não haja conteúdos, exibe uma mensagem no console
                    Console.WriteLine("Nenhum conteúdo encontrado.");
                }
            }
            else
            {
                // Caso o criador não seja encontrado, exibe uma mensagem no console
                Console.WriteLine("Criador não encontrado.");
            }
        }
    }
}
