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
    // P�gina para sele��o de conte�dos para a playlist
    public partial class SelecaoConteudos : ContentPage
    {
        // Atributos privados
        private ApiService _apiService;
        private int _CriadorId;
        private string _CriadorNome;
        private string _nomePlaylist;
        private int _idPlaylist;
        private List<int> conteudosSelecionados = new List<int>();  // Lista dos conte�dos selecionados

        // Construtor da p�gina
        public SelecaoConteudos(string nomePlaylist, int idPlaylist)
        {
            InitializeComponent();
            _apiService = new ApiService(); // Inicializa o servi�o de API
            _CriadorId = Preferences.Get("CriadorId", -1);  // Recupera o CriadorId armazenado
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o nome do Criador armazenado

            // Define o texto de boas-vindas
            bemVindo.Text = $"Criador: {_CriadorNome}";
            _nomePlaylist = nomePlaylist;

            // Exibe o nome da playlist na tela
            nomePlaylistLabel.Text = $"Playlist: {_nomePlaylist}";
            _idPlaylist = idPlaylist;
        }

        // M�todo para exibir os conte�dos na p�gina
        private async void ExibirConteudos(List<ConteudoDTO> conteudos)
        {
            MeuFlexLayout.Children.Clear(); // Limpa o FlexLayout



            foreach (var conteudo in conteudos)
            {

                // Obt�m o nome do criador associado ao CriadorId
                string criadorNome;
                try
                {
                    // Chama o m�todo que retorna um CriadorDTO
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

                // Valida o link de m�dia
                string midiaUrl = string.IsNullOrEmpty(conteudo.Link) || !conteudo.Link.StartsWith("https://")
                                  ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padr�o
                                  : conteudo.Link;

                // Verifica se a URL � do YouTube
                bool isYouTube = midiaUrl.Contains("youtu.be") || midiaUrl.Contains("youtube.com");

                string imageUrl;
                if (isYouTube)
                {
                    // Obt�m o ID do v�deo do YouTube
                    var videoId = UrlUtils.ExtractYouTubeVideoId(midiaUrl);
                    imageUrl = string.IsNullOrEmpty(videoId)
                               ? "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png" // URL padr�o
                               : $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";  // Thumbnail do v�deo
                }
                else
                {
                    imageUrl = midiaUrl; // Se n�o for YouTube, usa o link fornecido
                }

                // Cria o elemento de imagem para a m�dia
                var mediaElement = new Image
                {
                    Source = imageUrl,
                    Aspect = Aspect.AspectFill,
                    WidthRequest = 180,
                    HeightRequest = 100,
                    Margin = new Thickness(0, 8, 0, 0),
                    BackgroundColor = Colors.Transparent
                };

                // Cria o t�tulo do conte�do
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
                    MaxLines = 1 // Exibe apenas uma linha do t�tulo
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

                // Cria o bot�o de sele��o
                var button = new Button
                {
                    BorderColor = Colors.Transparent,
                    BorderWidth = 3,
                    CornerRadius = 12,
                    BackgroundColor = Colors.Transparent,
                };

                // Vari�vel para armazenar o estado de sele��o
                bool isSelected = false;

                // Evento de clique para o bot�o de sele��o
                button.Clicked += (sender, e) =>
                {
                    if (isSelected)
                    {
                        // Se j� estiver selecionado, desmarcar
                        isSelected = false;
                        button.BorderColor = Colors.Transparent;
                        conteudosSelecionados.Remove(conteudo.Id); // Remove da lista
                    }
                    else
                    {
                        // Se n�o estiver selecionado, marcar
                        isSelected = true;
                        button.BorderColor = Colors.Black;
                        button.BorderWidth = 3;
                        conteudosSelecionados.Add(conteudo.Id); // Adiciona � lista
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

        // M�todo para confirmar a sele��o dos conte�dos
        private async void ConfirmarSelecao()
        {
            if (conteudosSelecionados.Count > 0)
            {
                // Obtenha os conte�dos selecionados
                var conteudosSelecionadosDetalhados = await _apiService.GetAllConteudosAsync();
                var conteudosSelecionadosComTitulos = conteudosSelecionadosDetalhados
                    .Where(c => conteudosSelecionados.Contains(c.Id)) // Filtra os conte�dos selecionados
                    .Select(c => c.Titulo) // Pega apenas os t�tulos
                    .ToList();

                // Exibe a lista de t�tulos selecionados na DisplayAlert
                string titulosSelecionados = string.Join("\n", conteudosSelecionadosComTitulos);
                bool confirmar = await DisplayAlert(
                    "Confirmar Sele��o",
                    $"Voc� tem certeza que deseja adicionar os seguintes conte�dos � playlist?\n\n{titulosSelecionados}",
                    "Sim",
                    "N�o");

                // Se o usu�rio confirmar, prossegue com a adi��o dos conte�dos � playlist
                if (confirmar)
                {
                    try
                    {
                        // Chama o m�todo PostConteudosParaPlaylist passando os conte�dos selecionados
                        bool sucesso = await _apiService.PostConteudosParaPlaylist(_idPlaylist, conteudosSelecionados);

                        // Verifica o sucesso da opera��o
                        if (sucesso)
                        {
                            await DisplayAlert("Sucesso", "Conte�dos adicionados � playlist com sucesso!", "Continuar");
                            await Navigation.PopAsync(); // Navega de volta para a p�gina anterior
                        }
                        else
                        {
                            // Exibe mensagem de erro
                            await DisplayAlert("Erro", "Falha ao adicionar conte�dos � playlist.", "Continuar");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Exibe mensagem de erro em caso de exce��o
                        await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "Continuar");
                    }
                }
            }
            else
            {
                // Exibe mensagem se nenhum conte�do foi selecionado
                await DisplayAlert("Nenhuma Sele��o", "Nenhum conte�do foi selecionado.", "Continuar");
            }
        }




        // Evento de clique para confirmar a sele��o
        private void OnConfirmarSelecaoClicked(object sender, EventArgs e)
        {
            ConfirmarSelecao(); // Chama o m�todo para confirmar a sele��o
        }

        // Este m�todo � chamado quando a p�gina aparece na tela
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Obt�m todos os conte�dos dispon�veis
            var todosConteudos = await _apiService.GetAllConteudosAsync();

            // Busca a playlist pelo nome
            var playlist = await _apiService.GetPlaylistByNomeAsync(_nomePlaylist);

            // Verifica se a playlist foi encontrada
            if (playlist != null)
            {
                // Filtra os conte�dos que n�o est�o na playlist
                var conteudosDaPlaylist = playlist.Conteudos;
                var conteudosFiltrados = todosConteudos
                    .Where(c => !conteudosDaPlaylist.Any(p => p.Id == c.Id))  // Conte�dos n�o adicionados � playlist
                    .ToList();

                // Exibe os conte�dos filtrados
                ExibirConteudos(conteudosFiltrados);
            }
            else
            {
                // Se a playlist n�o for encontrada, exibe todos os conte�dos
                ExibirConteudos(todosConteudos);
            }
        }
    }
}
