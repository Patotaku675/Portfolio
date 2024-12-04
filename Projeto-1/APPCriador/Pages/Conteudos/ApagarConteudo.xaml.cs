using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

namespace APPCriador.Pages
{
    // Página para apagar conteúdo
    public partial class ApagarConteudo : ContentPage
    {
        // Atributo para armazenar o HttpClient, usado para fazer requisições HTTP
        private readonly HttpClient _httpClient;

        // Recupera o CriadorId armazenado nas preferências
        private readonly int _criadorId = Preferences.Get("CriadorId", -1);  // -1 é o valor padrão caso não haja ID armazenado

        // Atributo para armazenar o nome do Criador
        private string _CriadorNome;

        // Construtor da página
        public ApagarConteudo()
        {
            InitializeComponent();

            // Inicializa o HttpClient com a URL base da API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5245/") // URL base da API
            };

            // Recupera o nome do criador armazenado nas preferências
            _CriadorNome = Preferences.Get("CriadorNome", "Criador");

            // Atualiza o texto da página para mostrar o nome do criador
            bemVindo.Text = $"Criador: {_CriadorNome}";
        }

        // Método chamado quando o botão de apagar conteúdo por título é clicado
        private async void OnApagarPorTituloClicked(object sender, EventArgs e)
        {
            // Recupera o título do conteúdo que o usuário deseja apagar
            string titulo = TituloEntry.Text;

            // Verifica se o título foi informado
            if (string.IsNullOrEmpty(titulo))
            {
                await DisplayAlert("Apagar", "Por favor, insira um título válido.", "Continuar");
                return;
            }

            try
            {
                // Realiza uma requisição GET para buscar o conteúdo pelo título
                HttpResponseMessage getResponse = await _httpClient.GetAsync($"/api/Conteudo/titulo/{Uri.EscapeDataString(titulo)}");

                // Se a resposta não for bem-sucedida, exibe mensagem de erro
                if (!getResponse.IsSuccessStatusCode)
                {
                    await DisplayAlert("Apagar", "Conteúdo não encontrado.", "Continuar");
                    return;
                }

                // Obtém a resposta JSON e analisa usando JObject
                string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                var conteudo = JObject.Parse(jsonResponse);

                // Obtém os IDs do Criador e do conteúdo
                int criadorIdConteudo = conteudo["criadorId"]?.Value<int>() ?? -1;
                int conteudoId = conteudo["id"]?.Value<int>() ?? -1;

                // Verifica se o CriadorId do conteúdo é igual ao CriadorId do aplicativo
                if (criadorIdConteudo != _criadorId)
                {
                    await DisplayAlert("Apagar", "Você não tem permissão para apagar este conteúdo.", "Continuar");
                    return;
                }

                // Exibe uma caixa de confirmação para o usuário antes de apagar
                var confirm = await DisplayAlert("Confirmar",
                    $"Você tem certeza que deseja excluir o conteúdo '{titulo}'?",
                    "Sim", "Não");

                // Se o usuário confirmar, realiza a exclusão
                if (confirm)
                {
                    // Realiza a requisição DELETE para excluir o conteúdo pelo Id
                    HttpResponseMessage deleteResponse = await _httpClient.DeleteAsync($"/api/Conteudo/{conteudoId}");

                    // Verifica se a exclusão foi bem-sucedida
                    if (deleteResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Apagar", $"O Conteúdo: '{titulo}', foi apagado com sucesso!", "Continuar");
                        await Navigation.PopAsync(); // Retorna à página anterior
                    }
                    else
                    {
                        // Caso a exclusão falhe, exibe o erro
                        await DisplayAlert("Erro", $"Falha ao apagar o registro. Status: {deleteResponse.StatusCode}", "Continuar");
                    }
                }
            }
            catch (Exception ex)
            {
                // Se ocorrer um erro durante o processo, exibe uma mensagem de erro
                await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "Continuar");
            }
        }
    }
}
