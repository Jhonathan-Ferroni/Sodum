using Sodum.API.DTOs;
public interface ISteamService
{
    Task<GameDto?> SearchGameAsync(string gameName);
}