using Sodum.API.Models;
using System.ComponentModel.DataAnnotations;

namespace Sodum.API.Models
{
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string SpotifyUserId { get; set; } = string.Empty;

        // O SteamId costuma ser um número longo, mas tratamos como string para evitar overflow
        public string? SteamId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propriedade de navegação do Entity Framework
        public List<RecommendationSession> Sessions { get; set; } = new();
    }
}