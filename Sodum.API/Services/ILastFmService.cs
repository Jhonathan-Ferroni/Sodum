using Sodum.API.DTOs;

namespace Sodum.API.Services
{
    public interface ILastFmService
    {
        Task<MusicDto?> SearchTrackAsync(string trackName);
    }
}