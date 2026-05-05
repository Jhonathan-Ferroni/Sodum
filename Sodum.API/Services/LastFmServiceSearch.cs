using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Sodum.API.DTOs;

namespace Sodum.API.Services
{
    public class LastFmServiceSearch : ILastFmServiceSearch
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl; // Adicionamos a leitura do BaseUrl

        public LastFmServiceSearch(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Lê perfeitamente a estrutura do seu appsettings.json
            _apiKey = configuration["LastFm:ApiKey"];
            _baseUrl = configuration["LastFm:BaseUrl"]; 
        }

        public async Task<List<MusicDto>> SearchTracksSearchAsync(string trackName)
        {
            var result = new List<MusicDto>();

            // 1. Verificação rápida se você esqueceu de colar a chave no appsettings
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                Console.WriteLine("🚨 ALERTA: A API Key do Last.fm está vazia no appsettings.json!");
                return result;
            }

            try
            {
                // Monta a URL utilizando o seu BaseUrl
                var url = $"{_baseUrl}?method=track.search&track={Uri.EscapeDataString(trackName)}&api_key={_apiKey}&format=json&limit=5";
                
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync(); // Lemos o texto antes para logar se der erro

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    // 2. A API do Last.fm as vezes retorna Status 200, mas com uma mensagem de erro no JSON (ex: Invalid API Key)
                    if (root.TryGetProperty("error", out var errorElement))
                    {
                        var errorMsg = root.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Erro desconhecido";
                        Console.WriteLine($"❌ [Erro da API Last.fm]: {errorMsg}");
                        return result;
                    }

                    // 3. Navegação segura (Evita o erro se o Last.fm não achar a música e mandar o JSON vazio)
                    if (root.TryGetProperty("results", out var results) &&
                        results.TryGetProperty("trackmatches", out var trackmatches) &&
                        trackmatches.TryGetProperty("track", out var tracksArray))
                    {
                        foreach (var track in tracksArray.EnumerateArray())
                        {
                            result.Add(new MusicDto
                            {
                                Name = track.GetProperty("name").GetString(),
                                Artist = track.GetProperty("artist").GetString()
                            });
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ [Falha HTTP Last.fm]: Status {response.StatusCode} | Resposta: {json}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 [Exceção Last.fm Service]: {ex.Message}");
            }

            return result;
        }
    }
}