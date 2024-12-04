using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using APPCriador.Entities;

namespace APPCriador.Pages
{
    public partial class NovaPlaylist : ContentPage
    {
        private string _CriadorNome; // Nome do criador, que será exibido na tela

        // Construtor da página, inicializa os componentes e exibe o nome do criador
        public NovaPlaylist()
        {
            InitializeComponent();
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o nome do criador armazenado

            // Exibe o nome do criador na interface
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // Método chamado quando o botão "Adicionar" é clicado
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            // Recupera o CriadorId armazenado nas Preferências
            var _CriadorId = Preferences.Get("CriadorId", -1);  // -1 é o valor padrão caso não haja ID armazenado

            // Recupera o nome da playlist inserido no campo de entrada
            var nome = NomeEntry.Text;

            // Validação do campo de nome da playlist (não pode ser vazio)
            if (string.IsNullOrEmpty(nome))
            {
                await DisplayAlert("Criar", "Por favor, preencha o nome da playlist.", "Continuar");
                return;
            }

            // Criação do objeto para enviar à API, contendo o nome da playlist e o CriadorId
            var novaPlaylistInput = new
            {
                Nome = nome,
                CriadorId = _CriadorId // Passando o CriadorId armazenado nas preferências
            };

            // Envia o dado para a API para criar a playlist
            var client = new HttpClient(); // Criação de um cliente HTTP
            var content = new StringContent(JsonSerializer.Serialize(novaPlaylistInput), Encoding.UTF8, "application/json");

            // Adiciona cabeçalhos ao cliente HTTP, se necessário
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Realiza a requisição POST para a API
            var response = await client.PostAsync("http://localhost:5245/api/Playlists", content);

            // Verifica se a resposta da API foi bem-sucedida
            if (response.IsSuccessStatusCode)
            {
                // Obtém o conteúdo da resposta da API como string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Tenta deserializar o conteúdo da resposta em um objeto PlaylistDTO
                try
                {
                    var playlistCriada = JsonSerializer.Deserialize<PlaylistDTO>(responseContent);

                    if (playlistCriada != null)
                    {
                        // Se a playlist foi criada com sucesso, exibe uma mensagem de sucesso
                        await DisplayAlert("Criar", "Playlist adicionada com sucesso!", "Continuar");
                        await Navigation.PopAsync(); // Navega de volta ou para outra página
                    }
                    else
                    {
                        // Se a deserialização falhou, exibe uma mensagem de erro
                        await DisplayAlert("Erro", "Falha ao criar a playlist.", "Continuar");
                    }
                }
                catch (Exception ex)
                {
                    // Se ocorrer erro na deserialização, exibe a mensagem de erro
                    System.Diagnostics.Debug.WriteLine($"Erro de deserialização: {ex.Message}");
                    await DisplayAlert("Erro", "Falha ao deserializar a resposta da API", "Continuar");
                }
            }
            else
            {
                // Se a resposta da API não foi bem-sucedida, exibe a mensagem de erro
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", $"Falha ao criar playlist. Erro: {response.StatusCode}. Detalhes: {errorContent}", "Continuar");
            }
        }
    }
}
