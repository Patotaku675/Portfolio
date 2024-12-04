using APPCriador.Entities;
using APPCriador.Services;
using SkiaSharp;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Windows.Security.Cryptography.Core;

namespace APPCriador.Pages.Playlist
{
    public partial class PaginaVisualizacaoPlaylist : ContentPage
    {
        // Instância de serviço da API para realizar chamadas de rede
        private readonly ApiService _apiService;

        // Instância do HttpClient para realizar as requisições HTTP
        private readonly HttpClient _httpClient;

        // Armazena o ID e o nome do criador, retirados das preferências do usuário
        private readonly int _CriadorId;
        private readonly string _CriadorNome;

        // Armazena os dados da playlist atual
        private readonly int _playlistId;
        private readonly string _playlistNome;

        // Construtor da página, inicializa os campos e a interface
        public PaginaVisualizacaoPlaylist(string playlistNome, int playlistID)
        {
            InitializeComponent();

            // Inicializa os serviços
            _apiService = new ApiService();
            _httpClient = new HttpClient();

            // Obtém as preferências do criador (ID e nome)
            _CriadorId = Preferences.Get("CriadorId", -1);
            _CriadorNome = Preferences.Get("CriadorNome", "Criador");

            // Define o ID e nome da playlist
            _playlistId = playlistID;
            _playlistNome = playlistNome;

            // Atualiza a interface com os dados da playlist e criador
            playlistsNameLabel.Text = $"Playlist: {_playlistNome}";
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // Método que exibe os conteúdos da playlist
        private async void ExibirConteudos(List<ConteudoDTO> conteudos)
        {
            // Limpa os conteúdos anteriores exibidos
            MeuFlexLayout.Children.Clear();

            foreach (var conteudo in conteudos)
            {

                string criadorNome = _CriadorNome;

                // Cria um grid para cada conteúdo
                var grid = new Grid
                {
                    WidthRequest = 200,
                    HeightRequest = 220,
                    Margin = new Thickness(5),
                    BackgroundColor = Colors.Transparent
                };

                // Valida e ajusta o link de mídia
                string midiaUrl = string.IsNullOrEmpty(conteudo.Link) || !conteudo.Link.StartsWith("https://")
                                  ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padrão
                                  : conteudo.Link;

                // Verifica se o link é do YouTube
                bool isYouTube = midiaUrl.Contains("youtu.be") || midiaUrl.Contains("youtube.com");

                string imageUrl;
                if (isYouTube)
                {
                    // Se for YouTube, extrai o ID do vídeo e define a URL da miniatura
                    var videoId = UrlUtils.ExtractYouTubeVideoId(midiaUrl);
                    imageUrl = string.IsNullOrEmpty(videoId)
                               ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padrão
                               : $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";  // Thumbnail do vídeo
                }
                else
                {
                    // Se não for YouTube, usa a URL da mídia fornecida
                    imageUrl = midiaUrl;
                }

                // Cria o elemento de imagem para a mídia
                var mediaElement = new Image
                {
                    Source = imageUrl,
                    Aspect = Aspect.AspectFill,
                    WidthRequest = 180,
                    HeightRequest = 100,
                    Margin = new Thickness(0, 8, 0, 0),
                    BackgroundColor = Colors.Transparent
                };

                // Cria o label para o título do conteúdo
                var titleLabel = new Label
                {
                    Text = conteudo.Titulo,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    FontSize = 28,
                    Margin = new Thickness(0, 8, 0, 0),
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaxLines = 1
                };

                // Cria o botão de exclusão do conteúdo
                var minusButton = new Button
                {
                    Text = "-",
                    TextColor = Colors.Black,
                    FontSize = 30,
                    FontAttributes = FontAttributes.Bold,
                    BorderColor = Colors.Transparent,
                    BackgroundColor = Color.FromArgb("#ab8cc8"),
                    CornerRadius = 20,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(8),
                    Padding = new Thickness(0, -4, 0, 4)
                };

                // Cria o label exibindo o nome do criador do conteúdo
                var criadorLabel = new Label
                {
                    Text = $"C: {criadorNome}", // Exibe o nome do criador
                    VerticalOptions = LayoutOptions.End,
                    TranslationX = 50,
                    TranslationY = -14,
                    FontFamily = "Times New Roman",
                    FontSize = 24,
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    LineBreakMode = LineBreakMode.TailTruncation, // Impede quebra de linha
                    MaxLines = 1
                };

                // Cria o botão para abrir a visualização do conteúdo
                var button = new Button
                {
                    BorderColor = Colors.Transparent,
                    BorderWidth = 3,
                    CornerRadius = 12,
                    BackgroundColor = Colors.Transparent
                };

                // Evento de clique para abrir a página de visualização
                button.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new PaginaVisualizacao(midiaUrl, conteudo.Titulo));
                };

                // Evento de clique do botão de exclusão
                minusButton.Clicked += async (sender, e) =>
                {
                    var confirmacao = await DisplayAlert(
                        "Confirmar Exclusão",
                        $"Deseja realmente remover o conteúdo '{conteudo.Titulo}' da playlist '{_playlistNome}'?",
                        "Sim",
                        "Não");

                    if (confirmacao)
                    {
                        try
                        {
                            // Lista de IDs dos conteúdos para excluir
                            var conteudosParaExcluir = new List<int> { conteudo.Id };

                            // Chama a API para excluir o conteúdo
                            var sucesso = await _apiService.DeleteConteudosDaPlaylistAsync(_playlistId, conteudosParaExcluir);

                            if (sucesso)
                            {
                                await DisplayAlert("Sucesso", $"{conteudo.Titulo} foi removido da playlist.", "OK");
                                OnAppearing(); // Recarrega os conteúdos da playlist
                            }
                            else
                            {
                                await DisplayAlert("Erro", "Não foi possível excluir o conteúdo.", "OK");
                            }
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
                        }
                    }
                };

                // Adiciona todos os elementos no grid
                grid.Children.Add(mediaElement);
                grid.Children.Add(titleLabel);
                grid.Children.Add(criadorLabel);
                grid.Children.Add(button);
                grid.Children.Add(minusButton);

                // Adiciona o grid ao layout
                MeuFlexLayout.Children.Add(grid);
            }
        }

        // Método chamado quando a página é exibida, carrega os conteúdos da playlist
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Obtém a playlist pelo nome
            var playlist = await _apiService.GetPlaylistByNomeAsync(_playlistNome);

            // Verifica se o UsuarioId é nulo e ignora
            if (playlist != null && playlist.UsuarioId == null)
            {
                // Ignora o UsuarioId nulo (você pode não fazer nada ou logar um aviso, por exemplo)
                playlist.UsuarioId = null; // Garantir que o valor de UsuarioId seja nulo, se necessário
            }

            // Exibe os conteúdos da playlist, se houver
            if (playlist?.Conteudos != null && playlist.Conteudos.Count > 0)
            {
                ExibirConteudos(playlist.Conteudos);
            }
            else
            {
                MeuFlexLayout.Children.Clear(); // Limpa o layout caso não haja conteúdos
            }
        }
    }
}
