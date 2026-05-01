using Sodum.API.DTOs;
using System.Text.Json;

namespace Sodum.API.Services
{
    public class SteamService : ISteamService
    {
        private readonly HttpClient _httpClient;

        public SteamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Configurando a URL base pública da loja da Steam
            _httpClient.BaseAddress = new Uri("https://store.steampowered.com/api/");
        }

        public async Task<GameDto?> SearchGameAsync(string gameName)
        {
            var searchEndpoint = $"storesearch/?term={Uri.EscapeDataString(gameName)}&l=portuguese&cc=BR";
            var searchResponse = await _httpClient.GetAsync(searchEndpoint);
            if (!searchResponse.IsSuccessStatusCode) return null;

            var searchJson = await searchResponse.Content.ReadAsStringAsync();
            using var searchDoc = JsonDocument.Parse(searchJson);

            try
            {
                var items = searchDoc.RootElement.GetProperty("items");
                if (items.GetArrayLength() == 0) return null;

                var appId = items[0].GetProperty("id").GetInt32();
                var detailsEndpoint = $"appdetails?appids={appId}&l=portuguese&cc=BR"; // cc=BR garante o preço em Real
                var detailsResponse = await _httpClient.GetAsync(detailsEndpoint);
                var detailsJson = await detailsResponse.Content.ReadAsStringAsync();
                using var detailsDoc = JsonDocument.Parse(detailsJson);

                var data = detailsDoc.RootElement.GetProperty(appId.ToString()).GetProperty("data");

                // Lógica de extração de preço
                double finalPrice = 0;
                bool isFree = data.GetProperty("is_free").GetBoolean();

                if (!isFree && data.TryGetProperty("price_overview", out var priceElement))
                {
                    // A Steam entrega o valor em centavos (ex: 19900 para R$ 199,00)
                    int initialPriceInCents = priceElement.GetProperty("final").GetInt32();
                    finalPrice = initialPriceInCents / 100.0;
                }

                return new GameDto
                {
                    AppId = appId,
                    Name = data.GetProperty("name").GetString()!,
                    HeaderImage = data.GetProperty("header_image").GetString()!,
                    BackgroundImage = data.TryGetProperty("background_raw", out var bg) ? bg.GetString()! : "",
                    ShortDescription = data.GetProperty("short_description").GetString()!,
                    IsFree = isFree,
                    Price = finalPrice,

                    Genres = data.GetProperty("genres").EnumerateArray()
                                 .Select(g => g.GetProperty("description").GetString()!).ToList(),

                    Categories = data.GetProperty("categories").EnumerateArray()
                                     .Select(c => c.GetProperty("description").GetString()!).ToList(),

                    Developers = data.TryGetProperty("developers", out var devEl)
                                 ? devEl.EnumerateArray().Select(d => d.GetString()!).ToList()
                                 : new List<string>()
                };
            }
            catch
            {
                return null;
            }
        }
    }
}