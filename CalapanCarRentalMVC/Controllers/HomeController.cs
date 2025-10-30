using System.Diagnostics;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalapanCarRentalMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarRentalContext _context;

        public HomeController(ILogger<HomeController> logger, CarRentalContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get available cars for the home page
            var availableCars = await _context.Cars
                .Where(c => c.Status == "Available")
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(availableCars);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
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
