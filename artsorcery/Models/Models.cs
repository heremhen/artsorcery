using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace artsorcery.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [MaxLength(200)]
        public string? ProfilePicture { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Artwork> Artworks { get; set; }
    }

    public class Artwork
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist Artist { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int Views { get; set; }

        [Required]
        public int Likes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ArtworkImage> ArtworkImages { get; set; }

        public ICollection<ArtworkLike> ArtworkLikes { get; set; }
    }

    public class ArtworkImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ArtworkId { get; set; }

        [ForeignKey("ArtworkId")]
        public Artwork Artwork { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImagePath { get; set; }
    }

    public class ArtworkLike
    {
        [Required]
        public int ArtworkId { get; set; }

        [ForeignKey("ArtworkId")]
        public Artwork Artwork { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Artist User { get; set; }

        [Required]
        public DateTime LikedAt { get; set; }
    }
}