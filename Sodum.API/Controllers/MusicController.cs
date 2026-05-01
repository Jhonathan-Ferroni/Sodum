using Microsoft.AspNetCore.Mvc;
using Sodum.API.Services;
using System.Text.Json;

namespace Sodum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly ILastFmService _lastFmService;
        private readonly IRecommendationEngine _recommendationEngine;

        // Injetando o motor de IA aqui também
        public MusicController(ILastFmService lastFmService, IRecommendationEngine recommendationEngine)
        {
            _lastFmService = lastFmService;
            _recommendationEngine = recommendationEngine;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTrack([FromQuery] string trackName)
        {
            if (string.IsNullOrWhiteSpace(trackName)) return BadRequest("O nome da música não pode estar vazio.");
            var result = await _lastFmService.SearchTrackAsync(trackName);
            return result != null ? Ok(result) : NotFound("Música não encontrada.");
        }

        // A NOVA ROTA
        [HttpGet("recommend")]
        public async Task<IActionResult> RecommendGameForMusic([FromQuery] string trackName)
        {
            if (string.IsNullOrWhiteSpace(trackName))
                return BadRequest("Nome da música não informado.");

            // 1. Busca os dados da música no Last.fm
            var musicDto = await _lastFmService.SearchTrackAsync(trackName);

            if (musicDto == null)
                return NotFound("Música não encontrada no Last.fm.");

            // 2. Manda os dados para a IA pensar e gerar a lista de jogos
            var aiJsonResponse = await _recommendationEngine.GetGameRecommendationsForMusicAsync(musicDto);

            // 3. Devolve tudo mastigado para o React
            return Ok(new
            {
                Music = musicDto,
                Recommendations = JsonSerializer.Deserialize<object>(aiJsonResponse)
            });
        }
    }
}