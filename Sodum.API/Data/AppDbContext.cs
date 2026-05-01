using Microsoft.EntityFrameworkCore;
using Sodum.API.Models;

namespace Sodum.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Definindo as Tabelas
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RecommendationSession> RecommendationSessions { get; set; }
        public DbSet<RecommendedItem> RecommendedItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para garantir que se o usuário for deletado, 
            // as sessões dele também sejam (Cascade Delete)
            modelBuilder.Entity<UserProfile>()
                .HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento entre Sessão e Itens Recomendados
            modelBuilder.Entity<RecommendationSession>()
                .HasMany(s => s.SuggestedItems)
                .WithOne(i => i.Session)
                .HasForeignKey(i => i.RecommendationSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}