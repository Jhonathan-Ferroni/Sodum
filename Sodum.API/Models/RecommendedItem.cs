using Sodum.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sodum.API.Models
{
    public class RecommendedItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RecommendationSessionId { get; set; }

        [ForeignKey("RecommendationSessionId")]
        public RecommendationSession? Session { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        // ID original (Steam AppId ou Spotify TrackId) para renderizar links na tela depois
        public string ItemId { get; set; } = string.Empty;

        public string MatchReason { get; set; } = string.Empty;
    }
}