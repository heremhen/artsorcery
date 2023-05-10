using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace artsorcery.Models
{
    public class TokenUserMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }
    }
}
