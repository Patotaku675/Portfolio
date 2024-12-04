using APPCriador.Entities;
using APPCriador.Services;
using SkiaSharp;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace APPCriador.Pages.Playlist
{
    // Página para seleção de conteúdos para a playlist
    public partial class SelecaoConteudos : ContentPage
    {
        // Atributos privados
        private ApiService _apiService;
        private int _CriadorId;
        private string _CriadorNome;
        private string _nomePlaylist;
        private int _idPlaylist;
        private List<int> conteudosSelecionados = new List<int>();  // Lista dos conteúdos selecionados

        // Construtor da página
        public SelecaoConteudos(string nomePlaylist, int idPlaylist)
        {
            InitializeComponent();
            _apiService = new ApiService(); // Inicializa o serviço de API
            _CriadorId = Preferences.Get("CriadorId", -1);  // Recupera o CriadorId armazenado
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o nome do Criador armazenado

            // Define o texto de boas-vindas
            bemVindo.Text = $"Criador: {_CriadorNome}";
            _nomePlaylist = nomePlaylist;

            // Exibe o nome da playlist na tela
            nomePlaylistLabel.Text = $"Playlist: {_nomePlaylist}";
            _idPlaylist = idPlaylist;
        }

        // Método para exibir os conteúdos na página
        private async void ExibirConteudos(List<ConteudoDTO> conteudos)
        {
            MeuFlexLayout.Children.Clear(); // Limpa o FlexLayout



            foreach (var conteudo in conteudos)
            {

                // Obtém o nome do criador associado ao CriadorId
                string criadorNome;
                try
                {
                    // Chama o método que retorna um CriadorDTO
                    var criadorDto = await _apiService.GetCriadorByIdAsync(conteudo.CriadorId);

                    // Acessa o nome diretamente da propriedade do objeto retornado
                    criadorNome = criadorDto.Nome ?? "Desconhecido";
                }
                catch (Exception ex)
                {
                    criadorNome = "Erro ao carregar";
                    Console.WriteLine($"Erro ao obter o nome do criador: {ex.Message}");
                }



                // Cria um grid para organizar os elementos
                var grid = new Grid
                {
                    WidthRequest = 200,
                    HeightRequest = 160,
                    Margin = new Thickness(5),
                    BackgroundColor = Colors.Transparent
                };

                // Valida o link de mídia
                string midiaUrl = string.IsNullOrEmpty(conteudo.Link) || !conteudo.Link.StartsWith("https://")
                                  ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padrão
                                  : conteudo.Link;

                // Verifica se a URL é do YouTube
                bool isYouTube = midiaUrl.Contains("youtu.be") || midiaUrl.Contains("youtube.com");

                string imageUrl;
                if (isYouTube)
                {
                    // Obtém o ID do vídeo do YouTube
                    var videoId = UrlUtils.ExtractYouTubeVideoId(midiaUrl);
                    imageUrl = string.IsNullOrEmpty(videoId)
                               ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padrão
                               : $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";  // Thumbnail do vídeo
                }
                else
                {
                    imageUrl = midiaUrl; // Se não for YouTube, usa o link fornecido
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

                // Cria o título do conteúdo
                var titleLabel = new Label
                {
                    Text = conteudo.Titulo,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "Times New Roman",
                    FontSize = 28,
                    Margin = new Thickness(0, 0, 0, 0),
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    LineBreakMode = LineBreakMode.TailTruncation, // Impede quebra de linha
                    MaxLines = 1 // Exibe apenas uma linha do título
                };

                // Cria o label do criador
                var criadorLabel = new Label
                {
                    Text = $"C: {criadorNome}", // Exibe o nome do criador
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    FontFamily = "Times New Roman",
                    FontSize = 20,
                    Margin = new Thickness(10, 0, 0, 0),
                    TextColor = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    LineBreakMode = LineBreakMode.TailTruncation, // Impede quebra de linha
                    MaxLines = 1
                };

                // Cria o botão de seleção
                var button = new Button
                {
                    BorderColor = Colors.Transparent,
                    BorderWidth = 3,
                    CornerRadius = 12,
                    BackgroundColor = Colors.Transparent,
                };

                // Variável para armazenar o estado de seleção
                bool isSelected = false;

                // Evento de clique para o botão de seleção
                button.Clicked += (sender, e) =>
                {
                    if (isSelected)
                    {
                        // Se já estiver selecionado, desmarcar
                        isSelected = false;
                        button.BorderColor = Colors.Transparent;
                        conteudosSelecionados.Remove(conteudo.Id); // Remove da lista
                    }
                    else
                    {
                        // Se não estiver selecionado, marcar
                        isSelected = true;
                        button.BorderColor = Colors.Black;
                        button.BorderWidth = 3;
                        conteudosSelecionados.Add(conteudo.Id); // Adiciona à lista
                    }
                };

                // Adiciona os elementos ao grid
                grid.Children.Add(mediaElement);
                grid.Children.Add(titleLabel);
                grid.Children.Add(criadorLabel);
                grid.Children.Add(button);

                // Adiciona o grid ao FlexLayout
                MeuFlexLayout.Children.Add(grid);
            }
        }

        // Método para confirmar a seleção dos conteúdos
        private async void ConfirmarSelecao()
        {
            if (conteudosSelecionados.Count > 0)
            {
                // Obtenha os conteúdos selecionados
                var conteudosSelecionadosDetalhados = await _apiService.GetAllConteudosAsync();
                var conteudosSelecionadosComTitulos = conteudosSelecionadosDetalhados
                    .Where(c => conteudosSelecionados.Contains(c.Id)) // Filtra os conteúdos selecionados
                    .Select(c => c.Titulo) // Pega apenas os títulos
                    .ToList();

                // Exibe a lista de títulos selecionados na DisplayAlert
                string titulosSelecionados = string.Join("\n", conteudosSelecionadosComTitulos);
                bool confirmar = await DisplayAlert(
                    "Confirmar Seleção",
                    $"Você tem certeza que deseja adicionar os seguintes conteúdos à playlist?\n\n{titulosSelecionados}",
                    "Sim",
                    "Não");

                // Se o usuário confirmar, prossegue com a adição dos conteúdos à playlist
                if (confirmar)
                {
                    try
                    {
                        // Chama o método PostConteudosParaPlaylist passando os conteúdos selecionados
                        bool sucesso = await _apiService.PostConteudosParaPlaylist(_idPlaylist, conteudosSelecionados);

                        // Verifica o sucesso da operação
                        if (sucesso)
                        {
                            await DisplayAlert("Sucesso", "Conteúdos adicionados à playlist com sucesso!", "Continuar");
                            await Navigation.PopAsync(); // Navega de volta para a página anterior
                        }
                        else
                        {
                            // Exibe mensagem de erro
                            await DisplayAlert("Erro", "Falha ao adicionar conteúdos à playlist.", "Continuar");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Exibe mensagem de erro em caso de exceção
                        await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "Continuar");
                    }
                }
            }
            else
            {
                // Exibe mensagem se nenhum conteúdo foi selecionado
                await DisplayAlert("Nenhuma Seleção", "Nenhum conteúdo foi selecionado.", "Continuar");
            }
        }




        // Evento de clique para confirmar a seleção
        private void OnConfirmarSelecaoClicked(object sender, EventArgs e)
        {
            ConfirmarSelecao(); // Chama o método para confirmar a seleção
        }

        // Este método é chamado quando a página aparece na tela
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Obtém todos os conteúdos disponíveis
            var todosConteudos = await _apiService.GetAllConteudosAsync();

            // Busca a playlist pelo nome
            var playlist = await _apiService.GetPlaylistByNomeAsync(_nomePlaylist);

            // Verifica se a playlist foi encontrada
            if (playlist != null)
            {
                // Filtra os conteúdos que não estão na playlist
                var conteudosDaPlaylist = playlist.Conteudos;
                var conteudosFiltrados = todosConteudos
                    .Where(c => !conteudosDaPlaylist.Any(p => p.Id == c.Id))  // Conteúdos não adicionados à playlist
                    .ToList();

                // Exibe os conteúdos filtrados
                ExibirConteudos(conteudosFiltrados);
            }
            else
            {
                // Se a playlist não for encontrada, exibe todos os conteúdos
                ExibirConteudos(todosConteudos);
            }
        }
    }
}
