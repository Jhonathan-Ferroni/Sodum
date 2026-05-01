namespace Sodum.API.DTOs
{
    public class MusicDto
    {
        public string Name { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string? ArtistSummary { get; set; }
        public string? MusicUrl { get; set; }
    }
}