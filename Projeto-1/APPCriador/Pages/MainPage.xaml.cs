using APPCriador.Services;
using System.Linq;
using System.Threading.Tasks;
using APPCriador.Entities;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using APPCriador.Pages;

namespace APPCriador
{
    public partial class MainPage : Microsoft.Maui.Controls.ContentPage
    {
        private readonly ApiService _apiService;

        // Construtor da página
        public MainPage()
        {
            InitializeComponent();
            _apiService = new ApiService(); // Instancia o serviço da API

        }

        // Método executado ao clicar no botão "Cadastrar"
        private async void OnCadastrarClicked(object sender, EventArgs e)
        {
            var nomeCriador = NomeDoCriadorEntry?.Text; // Obtém o nome do criador digitado

            // Valida se o campo está vazio
            if (string.IsNullOrEmpty(nomeCriador))
            {
                await DisplayAlert("Cadastro", "Por favor, insira o nome do criador.", "Continuar");
                return;
            }

            // Obtém a lista de criadores existentes
            var criadores = await _apiService.GetAllCriadoresAsync();
            if (criadores == null)
            {
                await DisplayAlert("Erro", "Não foi possível obter a lista de criadores.", "Continuar");
                return;
            }

            // Verifica se o criador já existe
            var criadorExistente = criadores.FirstOrDefault(c => c?.Nome != null && c.Nome.Equals(nomeCriador, StringComparison.OrdinalIgnoreCase));

            if (criadorExistente != null)
            {
                await DisplayAlert("Cadastro", "Nome de Criador já existe.", "Continuar");
            }
            else
            {
                // Cria um novo criador e envia para a API
                var novoCriador = new CriadorDTO { Nome = nomeCriador };
                var criado = await _apiService.CreateCriadorAsync(novoCriador);

                if (criado != null)
                {
                    await DisplayAlert("Cadastro", $"O Criador {criado.Nome} foi adicionado.", "Continuar");
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao adicionar o criador. Tente novamente.", "Continuar");
                }
            }
        }

        // Método executado ao clicar no botão "Login"
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Login(); // Chama o método de login
        }

        // Método executado ao pressionar a tecla Enter no campo de entrada
        private async void OnEntryCompleted(object sender, EventArgs e)
        {
            await Login(); // Chama o método de login
        }

        // Método para realizar o login
        private async Task Login()
        {
            var nomeCriador = NomeDoCriadorEntry?.Text; // Obtém o nome do criador digitado

            // Valida se o campo está vazio
            if (string.IsNullOrWhiteSpace(nomeCriador))
            {
                await DisplayAlert("Login", "Por favor, insira o nome do criador.", "Continuar");
                return;
            }

            // Obtém a lista de criadores existentes
            var criadores = await _apiService.GetAllCriadoresAsync();
            if (criadores == null)
            {
                await DisplayAlert("Erro", "Não foi possível obter a lista de criadores.", "Continuar");
                return;
            }

            // Verifica se o criador existe na lista
            var criadorExistente = criadores.FirstOrDefault(c => c?.Nome != null && c.Nome.Equals(nomeCriador, StringComparison.OrdinalIgnoreCase));

            if (criadorExistente != null)
            {
                // Salva o ID do criador nas Preferências
                Preferences.Set("CriadorId", criadorExistente.Id);
                Preferences.Set("CriadorNome", criadorExistente.Nome);

                // Exibe uma mensagem e redireciona para a página do criador
                await DisplayAlert("Login", "Login bem-sucedido - Redirecionando", "Continuar");
                await Navigation.PushAsync(new PaginaDoCriador());
            }
            else
            {
                await DisplayAlert("Login", "Nome de Criador Não Existe", "Continuar");
            }
        }
    }
}
