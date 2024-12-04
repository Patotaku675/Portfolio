using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using APPCriador.Entities;
using Windows.Media.Playlists;

namespace APPCriador.Services
{
    // Serviço responsável por fazer requisições para a API
    public class ApiService
    {
        // Cliente HTTP para realizar requisições
        private readonly HttpClient _httpClient;

        // URL base da API
        private readonly string _baseUrl = "http://localhost:5245/api/criador";

        // Construtor da classe
        public ApiService()
        {
            _httpClient = new HttpClient(); // Inicializa o cliente HTTP
        }

        // MÉTODO PARA CRIAR UM CRIADOR (POST)
        public async Task<CriadorDTO?> CreateCriadorAsync(CriadorDTO criador)
        {
            try
            {
                // Converte o objeto CriadorDTO para JSON
                var json = JsonConvert.SerializeObject(criador);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envia a requisição POST para a API
                var response = await _httpClient.PostAsync(_baseUrl, content);

                // Verifica se a requisição foi bem-sucedida
                if (response.IsSuccessStatusCode)
                {
                    var createdCriador = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CriadorDTO>(createdCriador);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao criar criador: {response.StatusCode}, {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return null;
            }
        }

        // MÉTODO PARA ENVIAR CONTEÚDOS PARA UMA PLAYLIST (POST)
        public async Task<bool> PostConteudosParaPlaylist(int playlistId, List<int> conteudosIds)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5245/");

                // Define o cabeçalho para aceitar JSON
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Converte os IDs em uma query string (formato conteudosIds=1&conteudosIds=2&...)
                var queryString = string.Join("&", conteudosIds.Select(id => $"conteudosIds={id}"));
                var url = $"api/Playlists/{playlistId}/conteudos?{queryString}";  // Monta a URL com o playlistId e os conteudosIds

                // Envia o POST para a API
                var response = await client.PostAsync(url, null);  // Usamos 'null' já que não estamos enviando um corpo no request

                // Retorna true se o status da resposta for bem-sucedido (2xx)
                return response.IsSuccessStatusCode;
            }
        }



        // MÉTODOS PARA LEITURA DE DADOS (GET)

        // Retorna todos os criadores da API
        public async Task<List<CriadorDTO>?> GetAllCriadoresAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CriadorDTO>>(content);
                }

                return new List<CriadorDTO>();
            }
            catch
            {
                return null;
            }
        }

        // Retorna um criador pelo ID
        public async Task<CriadorDTO?> GetCriadorByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CriadorDTO>(content);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Retorna os conteúdos de uma playlist específica
        public async Task<List<ConteudoDTO>> GetConteudosByPlaylistIdAsync(int playlistId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5245/api/Playlists/{playlistId}/conteudos");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ConteudoDTO>>(content);
                }

                return new List<ConteudoDTO>();
            }
            catch
            {
                return new List<ConteudoDTO>();
            }
        }

        // Retorna playlists de um criador específico pelo ID
        public async Task<List<PlaylistDTO>> GetPlaylistsByCriadorIdAsync(int criadorId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5245/api/Playlists/criador/{criadorId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var playlists = JsonConvert.DeserializeObject<List<PlaylistDTO>>(content);

                    // Verifica e ajusta a resposta para garantir que o UsuarioId é tratado como nulo, se necessário
                    foreach (var playlist in playlists)
                    {
                        // Garantir que o UsuarioId seja tratado como nulo
                        if (playlist.UsuarioId == 0)
                        {
                            playlist.UsuarioId = null;
                        }

                        // Adiciona o PrimeiroConteudoLink se houver conteúdos
                        if (playlist.Conteudos != null && playlist.Conteudos.Any())
                        {
                            // Pega o link do primeiro conteúdo
                            playlist.PrimeiroConteudoLink = playlist.Conteudos.OrderBy(c => c.Id).FirstOrDefault()?.Link;
                        }
                        else
                        {
                            playlist.PrimeiroConteudoLink = "https://mnlht.com/wp-content/uploads/2017/06/no_image_placeholder.png"; // Link de fallback caso não haja conteúdo
                        }
                    }

                    return playlists;
                }

                return new List<PlaylistDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar playlists: {ex.Message}");
                return new List<PlaylistDTO>();
            }
        }




        public async Task<PlaylistDTO?> GetPlaylistByNomeAsync(string nome)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5245/api/Playlists/nome/{nome}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var playlist = JsonConvert.DeserializeObject<PlaylistDTO>(content);

                    // Se o UsuarioId for nulo, apenas ignore sem fazer nenhuma alteração
                    if (playlist != null && playlist.UsuarioId == null)
                    {
                        // Se necessário, você pode adicionar algum tipo de lógica aqui
                        // mas, no caso, estamos apenas ignorando o UsuarioId nulo
                    }

                    // Adiciona o PrimeiroConteudoLink se houver conteúdos
                    if (playlist != null && playlist.Conteudos != null && playlist.Conteudos.Any())
                    {
                        // Pega o link do primeiro conteúdo
                        playlist.PrimeiroConteudoLink = playlist.Conteudos.OrderBy(c => c.Id).FirstOrDefault()?.Link;
                    }

                    return playlist;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }



        // Retorna os conteúdos de um criador específico, incluindo links e curtidas
        public async Task<List<Content>> GetContentsByCreatorId(int criadorId)
        {
            var url = $"http://localhost:5245/api/criador/{criadorId}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var criadorResponseDto = JsonConvert.DeserializeObject<CriadorResponseDto>(json);  // Desserializa para o DTO

                // Verifica se os conteúdos existem e os mapeia para a lista de Content
                return criadorResponseDto?.Conteudos?.Select(c => new Content
                {
                    Id = c.Id,
                    Title = c.Titulo,
                    Link = c.Link,
                    Likes = c.Curtidas // Contagem de curtidas
                }).ToList() ?? new List<Content>();
            }

            return new List<Content>();
        }



        // MÉTODOS DE DELEÇÃO (DELETE)

        // Deleta uma playlist pelo ID
        public async Task<bool> DeletePlaylistAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5245/api/Playlists/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // MÉTODO PARA DELETAR CONTEÚDOS DE UMA PLAYLIST E RETORNAR SE FOI SUCESSO
        public async Task<bool> DeleteConteudosDaPlaylistAsync(int playlistId, List<int> conteudosIds)
        {
            try
            {
                // Converte a lista de IDs de conteúdos para um formato adequado para query string
                var conteudosIdsString = string.Join(",", conteudosIds);

                // Monta o endpoint da API com a query string de IDs de conteúdos
                var url = $"http://localhost:5245/api/Playlists/{playlistId}/conteudos?conteudosIds={conteudosIdsString}";

                // Faz a requisição DELETE para a URL com a query string
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // A operação foi bem-sucedida
                    return true;
                }
                else
                {
                    // Exibe mensagem de erro no console
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao deletar conteúdos: {response.StatusCode}, {errorContent}");
                    return false; // Retorna false em caso de erro
                }
            }
            catch (Exception ex)
            {
                // Exibe erro no console em caso de exceção
                Console.WriteLine($"Erro inesperado ao deletar conteúdos: {ex.Message}");
                return false; // Retorna false em caso de falha
            }
        }


        // Retorna todos os conteúdos da API
        public async Task<List<ConteudoDTO>> GetAllConteudosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5245/api/Conteudo");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ConteudoDTO>>(content);
                }

                return new List<ConteudoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar conteúdos: {ex.Message}");
                return new List<ConteudoDTO>();
            }
        }

        // Deleta um criador pelo ID
        public async Task<bool> DeleteCriadorAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
