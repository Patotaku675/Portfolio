using APPCriador.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace APPCriador.Pages
{
    public partial class ApagarPlaylist : ContentPage
    {
        private ApiService _apiService; // Serviço responsável por interagir com a API
        private string _CriadorNome; // Nome do criador que será exibido na interface

        // Construtor da página
        public ApagarPlaylist()
        {
            InitializeComponent();
            _apiService = new ApiService(); // Inicializa o serviço de API
            _CriadorNome = Preferences.Get("CriadorNome", "Criador"); // Recupera o nome do criador armazenado nas preferências
            bemVindo.Text = $"Criador: {_CriadorNome}"; // Exibe o nome do criador na interface
        }

        // Método chamado quando o botão "Apagar por Título" é clicado
        private async void OnApagarPorTituloClicked(object sender, EventArgs e)
        {
            // Obtém o nome da playlist inserido no campo de entrada, removendo espaços extras
            string nome = NomeEntry.Text?.Trim();

            // Verifica se o nome foi fornecido
            if (string.IsNullOrEmpty(nome))
            {
                // Exibe um alerta se o nome não foi informado
                await DisplayAlert("Apagar", "Por favor, insira o título da playlist para excluir.", "Continuar");
                return;
            }

            // Recupera o CriadorId armazenado nas preferências
            var criadorId = Preferences.Get("CriadorId", -1);

            // Tenta buscar a playlist pelo título usando o serviço da API
            var playlist = await _apiService.GetPlaylistByNomeAsync(nome);

            // Verifica se a playlist foi encontrada
            if (playlist != null)
            {
                // Exibe um alerta de confirmação para o usuário
                var confirm = await DisplayAlert("Confirmar",
                    $"Você tem certeza que deseja excluir a playlist '{nome}'?",
                    "Sim", "Não");

                // Se o usuário confirmar a exclusão
                if (confirm)
                {
                    // Tenta excluir a playlist pela API
                    var sucesso = await _apiService.DeletePlaylistAsync(playlist.Id);

                    if (sucesso)
                    {
                        // Exibe uma mensagem de sucesso e retorna à página anterior
                        await DisplayAlert("Apagar", "Playlist excluída com sucesso!", "Continuar");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        // Exibe uma mensagem de erro se a exclusão falhar
                        await DisplayAlert("Erro", "Falha ao excluir a playlist.", "Continuar");
                    }
                }
            }
            else
            {
                // Exibe uma mensagem se a playlist não for encontrada
                await DisplayAlert("Apagar", "Playlist não encontrada.", "Continuar");
            }
        }
    }
}
