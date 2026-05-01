namespace Sodum.API.DTOs
{
    public class GameDto
    {
        public int AppId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public List<string> Developers { get; set; } = new(); 
        public string HeaderImage { get; set; } = string.Empty;
        public string BackgroundImage { get; set; } = string.Empty; 
        public string ShortDescription { get; set; } = string.Empty;
        public bool IsFree { get; set; } 
        public double Price { get; set; }
    }
}