using Microsoft.Extensions.Logging; // Biblioteca para logs
using CommunityToolkit.Maui; // Toolkit para funcionalidades adicionais do MAUI
using CommunityToolkit.Maui.Core;

namespace APPCriador
{
    public static class MauiProgram
    {
        // Método principal para configurar e criar o aplicativo MAUI
        public static MauiApp CreateMauiApp()
        {
            // Inicializa o builder do aplicativo
            var builder = MauiApp.CreateBuilder();

            // Configurações principais do aplicativo
            builder
                .UseMauiApp<App>() // Define a classe principal do aplicativo
                .UseMauiCommunityToolkit() // Adiciona suporte ao Community Toolkit
                .UseMauiCommunityToolkitCore() // Registra funcionalidades centrais do Community Toolkit
                .ConfigureFonts(fonts => // Configuração das fontes personalizadas
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); // Fonte regular
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); // Fonte semibold
                });

#if DEBUG
            // Adiciona suporte a logs no modo de depuração
            builder.Logging.AddDebug();
#endif

            // Retorna o aplicativo configurado
            return builder.Build();
        }
    }
}
