using System.Diagnostics;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalapanCarRentalMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarRentalContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, CarRentalContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            // Get available cars for the home page (exclude cars in maintenance)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Send the contact message email
                    await _emailService.SendContactMessageAsync(
                        model.Name,
                        model.Email,
                        model.Phone ?? "",
                        model.Message
                    );

                    TempData["Success"] = "Thank you for contacting us! We'll get back to you soon.";
                    return RedirectToAction(nameof(Contact));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending contact message: {ex.Message}");
                    ModelState.AddModelError("", "There was an error sending your message. Please try again later or contact us directly.");
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TermsOfService()
        {
            return View();
        }

        public IActionResult Disclaimer()
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
