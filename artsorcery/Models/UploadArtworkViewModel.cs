using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace artsorcery.Models
{
    public class UploadArtworkViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Display(Name = "Images")]
        public List<IFormFile> ImageFiles { get; set; }
    }
}
