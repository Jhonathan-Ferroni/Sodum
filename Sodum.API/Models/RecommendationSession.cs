using Sodum.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sodum.API.Models
{
    public class RecommendationSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserProfileId { get; set; }

        [ForeignKey("UserProfileId")]
        public UserProfile? User { get; set; }

        // Para sabermos se o input foi um jogo ("Game") ou música ("Song")
        [Required]
        public string SourceType { get; set; } = string.Empty;

        // O nome do jogo ou da música que o usuário escolheu
        [Required]
        public string SourceItemName { get; set; } = string.Empty;

        // O ID original da Steam ou Spotify (útil se você quiser carregar a imagem da capa depois)
        public string SourceItemId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propriedade de navegação: 1 Sessão tem VÁRIOS itens recomendados
        public List<RecommendedItem> SuggestedItems { get; set; } = new();
    }
}