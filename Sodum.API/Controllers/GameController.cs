using Microsoft.AspNetCore.Mvc;
using Sodum.API.Services;
using System.Text.Json;

namespace Sodum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ISteamService _steamService;
        private readonly IRecommendationEngine _recommendationEngine;

        // Injetando a Steam e a IA no construtor
        public GameController(ISteamService steamService, IRecommendationEngine recommendationEngine)
        {
            _steamService = steamService;
            _recommendationEngine = recommendationEngine;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGame([FromQuery] string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return BadRequest("O nome do jogo não pode estar vazio.");

            var result = await _steamService.SearchGameAsync(gameName);
            return result != null ? Ok(result) : NotFound("Jogo não encontrado na Steam.");
        }

        // A ROTA MÁGICA DOS JOGOS
        [HttpGet("recommend")]
        public async Task<IActionResult> RecommendMusicForGame([FromQuery] string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return BadRequest("Nome do jogo não informado.");

            // 1. Busca os dados ricos na Steam
            var gameDto = await _steamService.SearchGameAsync(gameName);

            if (gameDto == null)
                return NotFound("Jogo não encontrado na Steam.");

            // 2. Manda os dados para a IA pensar e gerar a playlist
            var aiJsonResponse = await _recommendationEngine.GetMusicRecommendationsForGameAsync(gameDto);

            // 3. Devolve tudo mastigado para o React
            return Ok(new
            {
                Game = gameDto,
                Recommendations = JsonSerializer.Deserialize<object>(aiJsonResponse)
            });
        }
    }
}