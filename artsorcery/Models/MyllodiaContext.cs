using Microsoft.EntityFrameworkCore;

namespace artsorcery.Models
{
    public class MyllodiaContext : DbContext
    {
        public MyllodiaContext(DbContextOptions<MyllodiaContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<ArtworkImage> ArtworkImages { get; set; }
        public DbSet<ArtworkLike> ArtworkLikes { get; set; }
        public DbSet<TokenUserMapping> TokenUserMappings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-23O2NPG;Database=artsorcery;Trusted_Connection=True;User Id=sandy;Password=ruufii;TrustServerCertificate=true;");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtworkLike>()
                .HasKey(al => new { al.ArtworkId, al.UserId });

            modelBuilder.Entity<Artist>()
                .Property(a => a.Password)
                .HasMaxLength(100);

            modelBuilder.Entity<Artwork>()
                .HasOne(a => a.Artist)
                .WithMany(a => a.Artworks)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ArtworkImage>()
                .HasOne(ai => ai.Artwork)
                .WithMany(a => a.ArtworkImages)
                .HasForeignKey(ai => ai.ArtworkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArtworkLike>()
                .HasOne(al => al.Artwork)
                .WithMany(a => a.ArtworkLikes)
                .HasForeignKey(al => al.ArtworkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArtworkLike>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TokenUserMapping>()
                .HasKey(tum => tum.Token);
        }
    }
}
