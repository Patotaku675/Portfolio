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
        private string _CriadorNome; // Nome do criador, que ser� exibido na tela

        // Construtor da p�gina, inicializa os componentes e exibe o nome do criador
        public NovaPlaylist()
        {
            InitializeComponent();
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o nome do criador armazenado

            // Exibe o nome do criador na interface
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // M�todo chamado quando o bot�o "Adicionar" � clicado
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            // Recupera o CriadorId armazenado nas Prefer�ncias
            var _CriadorId = Preferences.Get("CriadorId", -1);  // -1 � o valor padr�o caso n�o haja ID armazenado

            // Recupera o nome da playlist inserido no campo de entrada
            var nome = NomeEntry.Text;

            // Valida��o do campo de nome da playlist (n�o pode ser vazio)
            if (string.IsNullOrEmpty(nome))
            {
                await DisplayAlert("Criar", "Por favor, preencha o nome da playlist.", "Continuar");
                return;
            }

            // Cria��o do objeto para enviar � API, contendo o nome da playlist e o CriadorId
            var novaPlaylistInput = new
            {
                Nome = nome,
                CriadorId = _CriadorId // Passando o CriadorId armazenado nas prefer�ncias
            };

            // Envia o dado para a API para criar a playlist
            var client = new HttpClient(); // Cria��o de um cliente HTTP
            var content = new StringContent(JsonSerializer.Serialize(novaPlaylistInput), Encoding.UTF8, "application/json");

            // Adiciona cabe�alhos ao cliente HTTP, se necess�rio
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Realiza a requisi��o POST para a API
            var response = await client.PostAsync("http://localhost:5245/api/Playlists", content);

            // Verifica se a resposta da API foi bem-sucedida
            if (response.IsSuccessStatusCode)
            {
                // Obt�m o conte�do da resposta da API como string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Tenta deserializar o conte�do da resposta em um objeto PlaylistDTO
                try
                {
                    var playlistCriada = JsonSerializer.Deserialize<PlaylistDTO>(responseContent);

                    if (playlistCriada != null)
                    {
                        // Se a playlist foi criada com sucesso, exibe uma mensagem de sucesso
                        await DisplayAlert("Criar", "Playlist adicionada com sucesso!", "Continuar");
                        await Navigation.PopAsync(); // Navega de volta ou para outra p�gina
                    }
                    else
                    {
                        // Se a deserializa��o falhou, exibe uma mensagem de erro
                        await DisplayAlert("Erro", "Falha ao criar a playlist.", "Continuar");
                    }
                }
                catch (Exception ex)
                {
                    // Se ocorrer erro na deserializa��o, exibe a mensagem de erro
                    System.Diagnostics.Debug.WriteLine($"Erro de deserializa��o: {ex.Message}");
                    await DisplayAlert("Erro", "Falha ao deserializar a resposta da API", "Continuar");
                }
            }
            else
            {
                // Se a resposta da API n�o foi bem-sucedida, exibe a mensagem de erro
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", $"Falha ao criar playlist. Erro: {response.StatusCode}. Detalhes: {errorContent}", "Continuar");
            }
        }
    }
}
