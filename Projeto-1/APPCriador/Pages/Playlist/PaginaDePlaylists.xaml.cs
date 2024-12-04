using APPCriador.Entities;
using APPCriador.Pages.Playlist;
using APPCriador.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APPCriador.Pages
{
    public partial class PaginaDePlaylists : ContentPage
    {
        private readonly ApiService _apiService; // Serviço para interação com a API
        private readonly int _CriadorId; // Identificador do criador
        private readonly string _CriadorNome; // Nome do criador

        // Construtor da página, inicializa os componentes e carrega os dados do criador
        public PaginaDePlaylists()
        {
            InitializeComponent();
            _apiService = new ApiService(); // Inicializa o serviço da API

            // Recupera os dados do criador a partir das preferências
            _CriadorId = Preferences.Get("CriadorId", -1);
            _CriadorNome = Preferences.Get("CriadorNome", "Criador");

            // Exibe o nome do criador na tela
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // Método para exibir as playlists no FlexLayout
        private void ExibirPlaylists(List<PlaylistDTO> playlists)
        {
            MeuFlexLayout1.Children.Clear(); // Limpa o layout antes de adicionar novas playlists

            // Itera sobre cada playlist e cria os elementos visuais
            foreach (var playlist in playlists)
            {
                var grid = new Grid
                {
                    WidthRequest = 200,
                    HeightRequest = 220, // Ajusta o tamanho para acomodar o conteúdo
                    Margin = new Thickness(5),
                    BackgroundColor = Colors.Transparent
                };

                // Título da playlist
                var titleLabel = new Label
                {
                    Text = playlist.Nome,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 36,
                    Margin = new Thickness(0, 8, 0, 0),
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    LineBreakMode = LineBreakMode.TailTruncation, // Impede a quebra de linha e corta o texto
                    MaxLines = 1 // Limita a exibição a uma linha
                };

                // Botão de adição (+) no canto inferior esquerdo
                var addButton = new Button
                {
                    Text = "+",
                    TextColor = Colors.Black,
                    FontSize = 30,
                    FontAutoScalingEnabled = true,
                    FontAttributes = FontAttributes.Bold,
                    BorderColor = Colors.Transparent,
                    BackgroundColor = Color.FromArgb("#ab8cc8"),
                    CornerRadius = 60,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(6)
                };

                // Evento de clique do botão de adicionar conteúdo à playlist
                addButton.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new SelecaoConteudos(playlist.Nome, playlist.Id));
                };

                // Contador de itens (conteúdos) na playlist no canto inferior direito
                var countLabel = new Label
                {
                    Text = playlist.Conteudos.Count.ToString(),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 32,
                    Margin = new Thickness(0, 0, 20, 10),
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent
                };

                // Obtém o link do primeiro conteúdo ou usa um link de imagem de fallback
                string previewLink = string.IsNullOrEmpty(playlist.PrimeiroConteudoLink)
                    ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png"
                    : playlist.PrimeiroConteudoLink;

                // Verifica se o link é válido e começa com "https://"
                string midiaUrl = string.IsNullOrEmpty(previewLink) || !previewLink.StartsWith("https://")
                                  ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png"
                                  : previewLink;

                // Identifica se o link é de um vídeo do YouTube
                string imageUrl = midiaUrl.Contains("youtu.be") || midiaUrl.Contains("youtube.com")
                    ? $"https://img.youtube.com/vi/{UrlUtils.ExtractYouTubeVideoId(midiaUrl)}/hqdefault.jpg"
                    : midiaUrl; // Se não for do YouTube, usa o link da imagem diretamente

                // Cria o componente de imagem para o preview
                var previewImage = new Image
                {
                    Source = imageUrl,
                    WidthRequest = 180,
                    HeightRequest = 100,
                    Aspect = Aspect.AspectFill,
                    Margin = new Thickness(0, 8, 10, 0),
                    BackgroundColor = Colors.Transparent
                };

                // Botão de visualização da playlist
                var button = new Button
                {
                    BorderColor = Colors.Transparent,
                    BorderWidth = 3,
                    CornerRadius = 12,
                    WidthRequest = 180,
                    HeightRequest = 100,
                    BackgroundColor = Colors.Transparent,
                };

                // Evento de clique para navegar para a página de visualização da playlist
                button.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new PaginaVisualizacaoPlaylist(playlist.Nome, playlist.Id));
                };

                // Adiciona os componentes ao grid da playlist
                grid.Children.Add(previewImage);
                grid.Children.Add(titleLabel);
                grid.Children.Add(countLabel);
                grid.Children.Add(button);
                grid.Children.Add(addButton);

                // Adiciona o grid ao FlexLayout da página
                MeuFlexLayout1.Children.Add(grid);
            }
        }

        // Ação do botão "Adicionar Playlist"
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovaPlaylist()); // Navega para a página de criação de nova playlist
        }

        // Ação do botão "Apagar Playlist"
        private async void OnApagarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ApagarPlaylist()); // Navega para a página de exclusão de playlists
        }

        // Método chamado quando a página aparece
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MeuFlexLayout1.Children.Clear(); // Limpa o FlexLayout

            int criadorId = _CriadorId;

            // Busca as playlists do criador usando o serviço da API
            var playlists = await _apiService.GetPlaylistsByCriadorIdAsync(criadorId);

            if (playlists != null && playlists.Count > 0)
            {
                // Exibe as playlists se houverem
                ExibirPlaylists(playlists);
            }
            else
            {
                Console.WriteLine("Nenhuma playlist encontrada.");
            }
        }
    }
}
