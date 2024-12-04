
namespace APPCriador.Pages
{
    public partial class PaginaDoCriador : ContentPage
    {
        private string _CriadorNome; // ID do criador que está armazenado nas preferências

        public PaginaDoCriador()
        {
            InitializeComponent();

            // Associa os eventos aos botões ao serem clicados
            conteudoButton.Clicked += ConteudoButton_Clicked;
            playlistButton.Clicked += PlaylistButton_Clicked;

            // Recupera o CriadorNome armazenado nas preferências
            _CriadorNome = Preferences.Get("CriadorNome", "Criador");
            //Define o texto da pagina
            bemVindo.Text = $"Bem Vindo: {_CriadorNome}";
        }

        // Método para abrir PaginaDeConteudos
        private async void ConteudoButton_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new PaginaDeConteudos());
        }

        // Método para abrir PaginaDePlaylists
        private async void PlaylistButton_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new PaginaDePlaylists());
        }




    }
}
