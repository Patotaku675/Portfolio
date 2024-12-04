using APPCriador.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace APPCriador.Pages
{
    public partial class ApagarPlaylist : ContentPage
    {
        private ApiService _apiService; // Servi�o respons�vel por interagir com a API
        private string _CriadorNome; // Nome do criador que ser� exibido na interface

        // Construtor da p�gina
        public ApagarPlaylist()
        {
            InitializeComponent();
            _apiService = new ApiService(); // Inicializa o servi�o de API
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o nome do criador armazenado nas prefer�ncias
            bemVindo.Text = $"Criador: {_CriadorNome}"; // Exibe o nome do criador na interface
        }

        // M�todo chamado quando o bot�o "Apagar por T�tulo" � clicado
        private async void OnApagarPorTituloClicked(object sender, EventArgs e)
        {
            // Obt�m o nome da playlist inserido no campo de entrada, removendo espa�os extras
            string nome = NomeEntry.Text?.Trim();

            // Verifica se o nome foi fornecido
            if (string.IsNullOrEmpty(nome))
            {
                // Exibe um alerta se o nome n�o foi informado
                await DisplayAlert("Apagar", "Por favor, insira o t�tulo da playlist para excluir.", "Continuar");
                return;
            }

            // Recupera o CriadorId armazenado nas prefer�ncias
            var criadorId = Preferences.Get("CriadorId", -1);

            // Tenta buscar a playlist pelo t�tulo usando o servi�o da API
            var playlist = await _apiService.GetPlaylistByNomeAsync(nome);

            // Verifica se a playlist foi encontrada
            if (playlist != null)
            {
                // Exibe um alerta de confirma��o para o usu�rio
                var confirm = await DisplayAlert("Confirmar",
                    $"Voc� tem certeza que deseja excluir a playlist '{nome}'?",
                    "Sim", "N�o");

                // Se o usu�rio confirmar a exclus�o
                if (confirm)
                {
                    // Tenta excluir a playlist pela API
                    var sucesso = await _apiService.DeletePlaylistAsync(playlist.Id);

                    if (sucesso)
                    {
                        // Exibe uma mensagem de sucesso e retorna � p�gina anterior
                        await DisplayAlert("Apagar", "Playlist exclu�da com sucesso!", "Continuar");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        // Exibe uma mensagem de erro se a exclus�o falhar
                        await DisplayAlert("Erro", "Falha ao excluir a playlist.", "Continuar");
                    }
                }
            }
            else
            {
                // Exibe uma mensagem se a playlist n�o for encontrada
                await DisplayAlert("Apagar", "Playlist n�o encontrada.", "Continuar");
            }
        }
    }
}
