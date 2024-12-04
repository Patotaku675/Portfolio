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
        private readonly ApiService _apiService; // Servi�o para intera��o com a API
        private readonly int _CriadorId; // Identificador do criador
        private readonly string _CriadorNome; // Nome do criador

        // Construtor da p�gina, inicializa os componentes e carrega os dados do criador
        public PaginaDePlaylists()
        {
            InitializeComponent();
            _apiService = new ApiService(); // Inicializa o servi�o da API

            // Recupera os dados do criador a partir das prefer�ncias
            _CriadorId = Preferences.Get("CriadorId", -1);
            _CriadorNome = Preferences.Get("CriadorNome", "Criador");

            // Exibe o nome do criador na tela
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // M�todo para exibir as playlists no FlexLayout
        private void ExibirPlaylists(List<PlaylistDTO> playlists)
        {
            MeuFlexLayout1.Children.Clear(); // Limpa o layout antes de adicionar novas playlists

            // Itera sobre cada playlist e cria os elementos visuais
            foreach (var playlist in playlists)
            {
                var grid = new Grid
                {
                    WidthRequest = 200,
                    HeightRequest = 220, // Ajusta o tamanho para acomodar o conte�do
                    Margin = new Thickness(5),
                    BackgroundColor = Colors.Transparent
                };

                // T�tulo da playlist
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
                    MaxLines = 1 // Limita a exibi��o a uma linha
                };

                // Bot�o de adi��o (+) no canto inferior esquerdo
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

                // Evento de clique do bot�o de adicionar conte�do � playlist
                addButton.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new SelecaoConteudos(playlist.Nome, playlist.Id));
                };

                // Contador de itens (conte�dos) na playlist no canto inferior direito
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

                // Obt�m o link do primeiro conte�do ou usa um link de imagem de fallback
                string previewLink = string.IsNullOrEmpty(playlist.PrimeiroConteudoLink)
                    ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png"
                    : playlist.PrimeiroConteudoLink;

                // Verifica se o link � v�lido e come�a com "https://"
                string midiaUrl = string.IsNullOrEmpty(previewLink) || !previewLink.StartsWith("https://")
                                  ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png"
                                  : previewLink;

                // Identifica se o link � de um v�deo do YouTube
                string imageUrl = midiaUrl.Contains("youtu.be") || midiaUrl.Contains("youtube.com")
                    ? $"https://img.youtube.com/vi/{UrlUtils.ExtractYouTubeVideoId(midiaUrl)}/hqdefault.jpg"
                    : midiaUrl; // Se n�o for do YouTube, usa o link da imagem diretamente

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

                // Bot�o de visualiza��o da playlist
                var button = new Button
                {
                    BorderColor = Colors.Transparent,
                    BorderWidth = 3,
                    CornerRadius = 12,
                    WidthRequest = 180,
                    HeightRequest = 100,
                    BackgroundColor = Colors.Transparent,
                };

                // Evento de clique para navegar para a p�gina de visualiza��o da playlist
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

                // Adiciona o grid ao FlexLayout da p�gina
                MeuFlexLayout1.Children.Add(grid);
            }
        }

        // A��o do bot�o "Adicionar Playlist"
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovaPlaylist()); // Navega para a p�gina de cria��o de nova playlist
        }

        // A��o do bot�o "Apagar Playlist"
        private async void OnApagarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ApagarPlaylist()); // Navega para a p�gina de exclus�o de playlists
        }

        // M�todo chamado quando a p�gina aparece
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MeuFlexLayout1.Children.Clear(); // Limpa o FlexLayout

            int criadorId = _CriadorId;

            // Busca as playlists do criador usando o servi�o da API
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
