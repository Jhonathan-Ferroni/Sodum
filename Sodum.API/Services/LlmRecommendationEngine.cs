using Sodum.API.DTOs;
using System.Text.Json;
using System.Text;

namespace Sodum.API.Services
{
    public class LlmRecommendationEngine : IRecommendationEngine
    {
        private readonly HttpClient _httpClient;

        public LlmRecommendationEngine(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            var apiKey = config["Groq:ApiKey"] ?? throw new ArgumentNullException("Groq ApiKey missing");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> GetMusicRecommendationsForGameAsync(GameDto game)
        {
            string prompt = $@"
                Analise o seguinte jogo:
                Nome: {game.Name}
                Desenvolvedores: {string.Join(", ", game.Developers)}
                Gêneros: {string.Join(", ", game.Genres)}
                Categorias: {string.Join(", ", game.Categories)}
                Descrição: {game.ShortDescription}
                Preço: R$ {game.Price:F2}

                Recomende 3 músicas reais para ouvir enquanto joga. 
                Pense no ritmo, atmosfera e público-alvo deste jogo.
                Responda APENAS com um array JSON válido, sem texto.
                Formato EXATO:
                [ {{ ""trackName"": ""..."", ""artistName"": ""..."", ""matchReason"": ""..."" }} ]";

            return await CallGroqAsync(prompt);
        }

        public async Task<string> GetGameRecommendationsForMusicAsync(MusicDto music)
        {
            string prompt = $@"
                Analise a seguinte música:
                Nome: {music.Name}
                Artista: {music.Artist}
                Tags/Gêneros: {string.Join(", ", music.Tags)}

                Recomende 3 jogos reais (disponíveis no mercado) que combinem perfeitamente com a energia, o clima e o tema desta música.
                Responda APENAS com um array JSON válido, sem texto.
                Formato EXATO:
                [ {{ ""gameName"": ""..."", ""developer"": ""..."", ""matchReason"": ""..."" }} ]";

            return await CallGroqAsync(prompt);
        }

        // Método privado para reaproveitar a chamada da API do Groq
        private async Task<string> CallGroqAsync(string prompt)
        {
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Você é um especialista em cultura pop, games e música. Retorne apenas JSON puro." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro na API do Groq. Status: {response.StatusCode} | Detalhes: {jsonResponse}");
            }

            using var doc = JsonDocument.Parse(jsonResponse);

            var aiMessage = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "[]";

            return aiMessage.Replace("```json", "").Replace("```", "").Trim();
        }
    }
}