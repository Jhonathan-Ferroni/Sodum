using Sodum.API.DTOs;
using System.Text.Json;

namespace Sodum.API.Services
{
    public class LastFmService : ILastFmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public LastFmService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Pegamos a URL base e a chave do appsettings
            _httpClient.BaseAddress = new Uri(configuration["LastFm:BaseUrl"]!);
            _apiKey = configuration["LastFm:ApiKey"] ?? throw new ArgumentNullException("LastFm ApiKey missing");
        }

        public async Task<MusicDto?> SearchTrackAsync(string trackName)
        {
            var searchEndpoint = $"?method=track.search&track={Uri.EscapeDataString(trackName)}&api_key={_apiKey}&format=json&limit=1";
            var searchResponse = await _httpClient.GetAsync(searchEndpoint);
            if (!searchResponse.IsSuccessStatusCode) return null;

            var searchJson = await searchResponse.Content.ReadAsStringAsync();
            using var searchDoc = JsonDocument.Parse(searchJson);

            try
            {
                var trackNode = searchDoc.RootElement.GetProperty("results").GetProperty("trackmatches").GetProperty("track")[0];
                var name = trackNode.GetProperty("name").GetString();
                var artist = trackNode.GetProperty("artist").GetString();

                var infoEndpoint = $"?method=track.getInfo&api_key={_apiKey}&artist={Uri.EscapeDataString(artist!)}&track={Uri.EscapeDataString(name!)}&format=json";
                var infoResponse = await _httpClient.GetAsync(infoEndpoint);
                var infoJson = await infoResponse.Content.ReadAsStringAsync();
                using var infoDoc = JsonDocument.Parse(infoJson);

                var trackDetails = infoDoc.RootElement.GetProperty("track");

                var dto = new MusicDto
                {
                    Name = name!,
                    Artist = artist!,
                    MusicUrl = trackDetails.GetProperty("url").GetString(),
                    Tags = trackDetails.GetProperty("toptags").GetProperty("tag")
                            .EnumerateArray().Select(t => t.GetProperty("name").GetString()!).ToList()
                };

                return dto;
            }
            catch { return null; }
        }
    }
}