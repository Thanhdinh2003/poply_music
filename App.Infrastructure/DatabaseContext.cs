using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<FavouriteCollection> FavouriteCollections { get; set; } = null!;
        public virtual DbSet<Media> Media { get; set; } = null!;
        public virtual DbSet<MediaContent> MediaContents { get; set; } = null!;
        public virtual DbSet<MediaViewHistory> MediaViewHistory { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<PremiumUpgradeRequest> PremiumUpgradeRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavouriteCollection>()
               .HasKey(e => new { e.UserId, e.MediaId });
        }

    }
}
