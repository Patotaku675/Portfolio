using System;
using System.Linq;

public static class UrlUtils
{
    /// <summary>
    /// Extrai o ID de um vídeo do YouTube a partir de uma URL.
    /// Suporta links curtos (youtu.be) e links completos (youtube.com).
    /// </summary>
    public static string? ExtractYouTubeVideoId(string url)
    {
        try
        {
            if (url.Contains("youtu.be"))
            {
                // Para links curtos (ex.: https://youtu.be/abc123), o ID está após o último "/".
                return url.Split('/').LastOrDefault(); // Usa LastOrDefault para evitar exceções em caso de URL inválida.
            }
            else if (url.Contains("youtube.com"))
            {
                // Para links completos (ex.: https://www.youtube.com/watch?v=abc123), o ID está associado ao parâmetro "v".
                var query = System.Web.HttpUtility.ParseQueryString(new Uri(url).Query);
                return query.Get("v"); // Retorna null se o parâmetro "v" não existir.
            }
        }
        catch
        {
            // Retorna null em caso de erro (ex.: URL malformada).
        }

        return null; // Retorna null caso nenhuma condição seja atendida.
    }
}
