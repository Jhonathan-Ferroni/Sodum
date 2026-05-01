using Sodum.API.DTOs;

namespace Sodum.API.Services
{
    public interface IRecommendationEngine
    {
        Task<string> GetMusicRecommendationsForGameAsync(GameDto game);
        Task<string> GetGameRecommendationsForMusicAsync(MusicDto music);
    }
}
