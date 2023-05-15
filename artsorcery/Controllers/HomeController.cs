using artsorcery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace artsorcery.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyllodiaContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MyllodiaContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //public IActionResult Index()
        //{
        //    ViewData["Title"] = "Home Page";
        //    var last10Artworks = _context.Artworks
        //        .OrderByDescending(a => a.CreatedAt)
        //        .Include(a => a.ArtworkImages)
        //        .Take(10)
        //        .ToList();

        //    ViewBag.Artworks = last10Artworks;

        //    return View();
        //}

        public IActionResult Index(int page = 1)
        {
            ViewData["Title"] = "Home Page";
            int pageSize = 10;

            var artworksCount = _context.Artworks.Count();
            var totalPages = (int)Math.Ceiling((double)artworksCount / pageSize);

            var artworks = _context.Artworks
                .OrderByDescending(a => a.CreatedAt)
                .Include(a => a.ArtworkImages)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Artworks = artworks;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}