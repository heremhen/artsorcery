using artsorcery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace artsorcery.Controllers
{
    public class ArtistController : Controller
    {
        private readonly MyllodiaContext _context;

        public ArtistController(MyllodiaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Artist/Register
        public IActionResult Register()
        {
            if (HttpContext.Request.Cookies.ContainsKey("AuthToken"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Artist/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username, Email, Password")] Artist artist)
        {
            if (!ModelState.IsValid)
            {
                if (await _context.Artists.AnyAsync(a => a.Username == artist.Username))
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(artist);
                }

                if (await _context.Artists.AnyAsync(a => a.Email == artist.Email))
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(artist);
                }

                artist.CreatedAt = DateTime.Now;
                _context.Artists.Add(artist);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
                }
            }
            return View(artist);
        }

        // GET: Artist/Login
        public IActionResult Login()
        {
            if (HttpContext.Request.Cookies.ContainsKey("AuthToken"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private void SaveTokenForUser(string token, int userId)
        {
            var mapping = new TokenUserMapping
            {
                Token = token,
                ArtistId = userId
            };

            _context.TokenUserMappings.Add(mapping);
            _context.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username, Password")] Artist artist)
        {
            if (!ModelState.IsValid)
            {
                var authenticatedArtist = await _context.Artists.FirstOrDefaultAsync(a => a.Username == artist.Username && a.Password == artist.Password);
                if (authenticatedArtist != null)
                {
                    var token = Guid.NewGuid().ToString();

                    SaveTokenForUser(token, authenticatedArtist.Id);

                    HttpContext.Response.Cookies.Append("AuthToken", token);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return View(artist);
        }

        public IActionResult Profile()
        {
            var authToken = HttpContext.Request.Cookies["AuthToken"];
            var mapping = _context.TokenUserMappings.FirstOrDefault(t => t.Token == authToken);

            if (mapping == null)
            {
                return NotFound();
            }

            var artist = _context.Artists.Find(mapping.ArtistId);

            if (artist == null)
            {
                return NotFound();
            }

            var artworks = _context.Artworks.Where(a => a.ArtistId == artist.Id).ToList();

            var viewModel = new ArtistProfileViewModel
            {
                Username = artist.Username,
                Email = artist.Email,
                ProfilePicture = artist?.ProfilePicture,
                Artworks = artworks
            };

            return View(viewModel);
        }

        //public IActionResult Profile(int id)
        //{
        //    var artist = _context.Artists.Find(id);

        //    if (artist == null)
        //    {
        //        return NotFound();
        //    }

        //    var viewModel = new ArtistProfileViewModel
        //    {
        //        Id = artist.Id,
        //        Username = artist.Username,
        //        Email = artist.Email,
        //        ProfilePicture = artist.ProfilePicture
        //    };

        //    ViewBag.ArtistId = viewModel.Id;

        //    return View(viewModel);
        //}

        public IActionResult Settings()
        {
            var authToken = HttpContext.Request.Cookies["AuthToken"];
            var mapping = _context.TokenUserMappings.FirstOrDefault(t => t.Token == authToken);

            if (mapping != null)
            {
                var artist = _context.Artists.Find(mapping.ArtistId);
                if (artist != null)
                {
                    return View(artist);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Settings(Artist artist, IFormFile profilePicture)
        {
            var authToken = HttpContext.Request.Cookies["AuthToken"];
            var mapping = _context.TokenUserMappings.FirstOrDefault(t => t.Token == authToken);

            if (mapping != null)
            {
                var existingArtist = await _context.Artists.FindAsync(mapping.ArtistId);
                if (existingArtist != null)
                {
                    // Update the artist fields based on the submitted values
                    existingArtist.Username = artist.Username;
                    existingArtist.Email = artist.Email;

                    // Check if a new password was provided and update it accordingly
                    if (!string.IsNullOrEmpty(artist.Password))
                    {
                        existingArtist.Password = artist.Password;
                    }

                    // Process the profile picture if it is uploaded
                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        // Generate a unique filename for the profile picture
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
                        var imagePath = Path.Combine("wwwroot", "public", "images", fileName);

                        // Save the uploaded profile picture to the specified location
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await profilePicture.CopyToAsync(fileStream);
                        }

                        // Update the artist's profile picture path
                        existingArtist.ProfilePicture = "/public/images/" + fileName;
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Profile", new { id = existingArtist.Id });
                }
            }

            // If the mapping or artist is not found, handle accordingly
            return NotFound();
        }
    }
}
