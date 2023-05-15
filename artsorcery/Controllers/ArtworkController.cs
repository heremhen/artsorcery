using artsorcery.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace artsorcery.Controllers
{
    public class ArtworkController : Controller
    {
        private readonly MyllodiaContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ArtworkController(MyllodiaContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        private string SaveImageFile(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var imagePath = Path.Combine("images", fileName);

            var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "public", "images");

            Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return imagePath;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            var model = new UploadArtworkViewModel();
            return View(model);
        }

        public int GetArtistIdFromToken()
        {
            var authToken = HttpContext.Request.Cookies["AuthToken"];
            var mapping = _context.TokenUserMappings.FirstOrDefault(t => t.Token == authToken);

            if (mapping != null)
            {
                return mapping.ArtistId;
            }

            return -1;
        }

        public IActionResult Search(string searchQuery, int page = 1)
        {
            ViewData["Title"] = "Search Results";

            // Retrieve the artworks matching the search query
            var searchResults = _context.Artworks
                .Where(a => a.Title.Contains(searchQuery))
                .Include(a => a.ArtworkImages)
                .ToList();

            // Calculate pagination values
            int pageSize = 10;
            int skipAmount = (page - 1) * pageSize;
            int totalArtworks = searchResults.Count();
            int totalPages = (int)Math.Ceiling((double)totalArtworks / pageSize);

            // Retrieve the artworks for the current page
            var artworks = searchResults
                .Skip(skipAmount)
                .Take(pageSize)
                .ToList();

            // Pass the data to the view
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Artworks = artworks;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadArtworkViewModel model)
        {
            if (ModelState.IsValid)
            {
                var artistId = GetArtistIdFromToken();
                if (artistId == -1)
                {
                    return View(model);
                }
                var artwork = new Artwork
                {
                    ArtistId = artistId,
                    Title = model.Title,
                    Description = model.Description,
                    Views = 0,
                    Likes = 0,
                    CreatedAt = DateTime.Now
                };

                _context.Artworks.Add(artwork);
                await _context.SaveChangesAsync();

                foreach (var file in model.ImageFiles)
                {
                    var imagePath = SaveImageFile(file);

                    var artworkImage = new ArtworkImage
                    {
                        ArtworkId = artwork.Id,
                        ImagePath = imagePath
                    };

                    _context.ArtworkImages.Add(artworkImage);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Artwork", new { id = artwork.Id });
            }

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var artwork = _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.ArtworkImages)
                .FirstOrDefault(a => a.Id == id);

            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        public IActionResult Edit(int id)
        {
            var artwork = _context.Artworks.Find(id);

            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        [HttpPost]
        public IActionResult Edit(Artwork artwork)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(artwork);
            //}

            var existingArtwork = _context.Artworks.Find(artwork.Id);

            if (existingArtwork == null)
            {
                return NotFound();
            }

            existingArtwork.Title = artwork.Title;
            existingArtwork.Description = artwork.Description;

            _context.SaveChanges();

            return RedirectToAction("Profile", "Artist");
        }


        public IActionResult Delete(int id)
        {
            var artwork = _context.Artworks.Find(id);

            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var artwork = _context.Artworks.Find(id);

            if (artwork == null)
            {
                return NotFound();
            }

            _context.Artworks.Remove(artwork);
            _context.SaveChanges();

            return RedirectToAction("Profile", "Artist");
        }

    }
}
