using System.Text;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using APPCriador.Entities;

namespace APPCriador.Pages
{
    // Página para adicionar um novo conteúdo
    public partial class NovoConteudo : ContentPage
    {
        // Atributo para armazenar o nome do Criador
        private string _CriadorNome;

        // Construtor da página
        public NovoConteudo()
        {
            InitializeComponent();

            // Recupera o nome do criador armazenado nas preferências
            _CriadorNome = Preferences.Get("CriadorNome", "Criador");

            // Atualiza o texto da página para mostrar o nome do criador
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // Método chamado quando o botão de adicionar conteúdo é clicado
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            // Recupera o CriadorId armazenado nas Preferências
            var _CriadorId = Preferences.Get("CriadorId", -1);  // -1 é o valor padrão caso não haja ID armazenado

            // Recupera os valores preenchidos nos campos de título e link
            var titulo = tituloEntry.Text;
            var link = linkEntry.Text;

            // Validação dos campos, caso algum esteja vazio, exibe uma mensagem de erro
            if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(link))
            {
                await DisplayAlert("Upload", "Por favor, preencha todos os campos.", "Continuar");
                return;
            }

            // Cria um modelo anônimo para enviar os dados para a API
            var conteudoInput = new
            {
                Titulo = titulo,
                Link = link,
                CriadorId = _CriadorId // Passa o CriadorId recuperado das preferências
            };

            // Cria um cliente HTTP para enviar a requisição
            var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(conteudoInput), Encoding.UTF8, "application/json");

            // Adiciona cabeçalhos à requisição, se necessário
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Envia a requisição para a API para criar o conteúdo
            var response = await client.PostAsync("http://localhost:5245/api/Conteudo", content);

            // Verifica se a resposta foi bem-sucedida
            if (response.IsSuccessStatusCode)
            {
                // Obtém o conteúdo da resposta como string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Tenta deserializar a resposta em um objeto ConteudoDTO
                try
                {
                    var conteudoCriado = JsonSerializer.Deserialize<ConteudoDTO>(responseContent);

                    // Se o conteúdo foi criado, exibe uma mensagem de sucesso
                    if (conteudoCriado != null)
                    {
                        await DisplayAlert("Upload", "Conteúdo adicionado com sucesso!", "OK");
                        await Navigation.PopAsync(); // Navega de volta para a página anterior
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Falha ao fazer o upload do conteúdo.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    // Se ocorrer erro durante a deserialização, exibe a mensagem de erro
                    System.Diagnostics.Debug.WriteLine($"Erro de deserialização: {ex.Message}");
                    await DisplayAlert("Error 404", "Falha ao deserializar a resposta da API", "Continuar");
                }
            }
            else
            {
                // Se a resposta não for bem-sucedida, exibe a mensagem de erro
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", $"Falha ao criar conteúdo. Erro: {response.StatusCode}. Detalhes: {errorContent}", "Continuar");
            }
        }
    }
}
