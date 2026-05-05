using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Sodum.API.DTOs;
using Sodum.API.Services; // Lembre-se de verificar o namespace da sua nova interface

namespace Sodum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        // Atualizado para a nova interface que retorna a lista
        private readonly ILastFmServiceSearch _lastFmServiceSearch;

        // Injeção de dependência atualizada
        public SearchController(ILastFmServiceSearch lastFmServiceSearch)
        {
            _lastFmServiceSearch = lastFmServiceSearch;
        }

       // ... (usings e injeção de dependência iguais) ...

[HttpGet("autocomplete")]
// ADICIONE O PARÂMETRO 'tipo' AQUI:
public async Task<IActionResult> Autocomplete([FromQuery] string q, [FromQuery] string tipo)
{
    if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
    {
        return Ok(new List<SuggestionDTO>());
    }

    var sugestoes = new List<SuggestionDTO>();
    
    // Normaliza para evitar erros se o React mandar com letra maiúscula
    var tipoBusca = tipo?.ToLower() ?? "";

    // 1. SÓ BUSCA JOGOS SE O TIPO FOR "jogo"
    if (tipoBusca == "jogo")
    {
        try
        {
            var urlSteam = $"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(q)}&l=portuguese&cc=BR";
            var responseSteam = await _httpClient.GetAsync(urlSteam);

            if (responseSteam.IsSuccessStatusCode)
            {
                var jsonSteam = await responseSteam.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonSteam);
                
                if (doc.RootElement.TryGetProperty("items", out var itemsElement))
                {
                    var games = itemsElement.EnumerateArray().Take(5);
                    foreach (var game in games)
                    {
                        sugestoes.Add(new SuggestionDTO
                        {
                            Id = game.GetProperty("id").GetInt32().ToString(),
                            Nome = game.GetProperty("name").GetString(),
                            Tipo = "Jogo"
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro Steam: {ex.Message}");
        }
    }

    // 2. SÓ BUSCA MÚSICAS SE O TIPO FOR "musica"
    if (tipoBusca == "musica")
    {
        try
        {
            var musicResults = await _lastFmServiceSearch.SearchTracksSearchAsync(q);
            if (musicResults != null && musicResults.Any())
            {
                foreach (var music in musicResults.Take(5))
                {
                    sugestoes.Add(new SuggestionDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nome = $"{music.Name} - {music.Artist}", 
                        Tipo = "Música"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro LastFm Service Search: {ex.Message}");
        }
    }

    return Ok(sugestoes);
}
    }
}