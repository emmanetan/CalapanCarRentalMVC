using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;

namespace CalapanCarRentalMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly CarRentalContext _context;

        public AdminController(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Check if user is logged in as admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // Get statistics
            ViewBag.TotalCars = await _context.Cars.CountAsync();
            ViewBag.AvailableCars = await _context.Cars.CountAsync(c => c.Status == "Available");
            ViewBag.RentedCars = await _context.Cars.CountAsync(c => c.Status == "Rented");
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.ActiveRentals = await _context.Rentals.CountAsync(r => r.Status == "Active");
            ViewBag.TotalRevenue = await _context.Rentals
          .Where(r => r.Status == "Completed")
       .SumAsync(r => r.TotalAmount);

            // Recent rentals
            ViewBag.RecentRentals = await _context.Rentals
               .Include(r => r.Car)
              .Include(r => r.Customer)
                 .OrderByDescending(r => r.CreatedAt)
              .Take(5)
             .ToListAsync();

            return View();
        }
    }
}
