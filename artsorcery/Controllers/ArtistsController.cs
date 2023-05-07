using artsorcery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
                // Check if the username already exists
                if (await _context.Artists.AnyAsync(a => a.Username == artist.Username))
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(artist);
                }

                // Check if the email already exists
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
                    HttpContext.Response.Cookies.Append("AuthToken", token);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return View(artist);
        }

    }
}