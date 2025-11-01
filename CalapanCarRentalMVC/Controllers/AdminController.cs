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

         // Maintenance statistics
            ViewBag.UrgentMaintenance = await _context.Maintenances.CountAsync(m => m.Status == "Urgent");
    ViewBag.ScheduledMaintenance = await _context.Maintenances.CountAsync(m => m.Status == "Scheduled");
        ViewBag.CompletedMaintenance = await _context.Maintenances
       .CountAsync(m => m.Status == "Completed" && 
      m.DateCompleted.HasValue && 
       m.DateCompleted.Value.Month == DateTime.Now.Month &&
        m.DateCompleted.Value.Year == DateTime.Now.Year);

            // Report Trends - Monthly data
       var currentMonth = DateTime.Now.Month;
   var currentYear = DateTime.Now.Year;
   var lastMonth = DateTime.Now.AddMonths(-1);

   // This Month
         ViewBag.ThisMonthRentals = await _context.Rentals
    .CountAsync(r => r.RentalDate.Month == currentMonth && r.RentalDate.Year == currentYear);
   
    ViewBag.ThisMonthRevenue = await _context.Rentals
      .Where(r => r.RentalDate.Month == currentMonth && r.RentalDate.Year == currentYear)
      .SumAsync(r => (decimal?)r.TotalAmount) ?? 0;

            // Last Month
       ViewBag.LastMonthRentals = await _context.Rentals
          .CountAsync(r => r.RentalDate.Month == lastMonth.Month && r.RentalDate.Year == lastMonth.Year);
   
          ViewBag.LastMonthRevenue = await _context.Rentals
     .Where(r => r.RentalDate.Month == lastMonth.Month && r.RentalDate.Year == lastMonth.Year)
         .SumAsync(r => (decimal?)r.TotalAmount) ?? 0;

            // Calculate percentage changes
            ViewBag.RentalsChange = ViewBag.LastMonthRentals > 0 
           ? Math.Round(((ViewBag.ThisMonthRentals - ViewBag.LastMonthRentals) / (double)ViewBag.LastMonthRentals) * 100, 1)
        : 0;
            
       ViewBag.RevenueChange = ViewBag.LastMonthRevenue > 0 
      ? Math.Round(((ViewBag.ThisMonthRevenue - ViewBag.LastMonthRevenue) / ViewBag.LastMonthRevenue) * 100, 1)
    : 0;

    // This Year
         ViewBag.ThisYearRentals = await _context.Rentals
                .CountAsync(r => r.RentalDate.Year == currentYear);
            
            ViewBag.ThisYearRevenue = await _context.Rentals
          .Where(r => r.RentalDate.Year == currentYear)
         .SumAsync(r => (decimal?)r.TotalAmount) ?? 0;

     // Monthly data for chart (last 6 months)
   var monthlyRentalCounts = new List<int>();
  var monthlyRevenue = new List<decimal>();
            var monthLabels = new List<string>();

  for (int i = 5; i >= 0; i--)
        {
             var date = DateTime.Now.AddMonths(-i);
          var count = await _context.Rentals
      .CountAsync(r => r.RentalDate.Month == date.Month && r.RentalDate.Year == date.Year);
     var revenue = await _context.Rentals
                    .Where(r => r.RentalDate.Month == date.Month && r.RentalDate.Year == date.Year)
          .SumAsync(r => (decimal?)r.TotalAmount) ?? 0;

          monthlyRentalCounts.Add(count);
    monthlyRevenue.Add(revenue);
          monthLabels.Add(date.ToString("MMM"));
    }

       ViewBag.MonthlyRentalCounts = monthlyRentalCounts;
ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.MonthLabels = monthLabels;

      return View();
        }

        // GET: Admin/Profile
        public async Task<IActionResult> Profile()
  {
            var userRole = HttpContext.Session.GetString("UserRole");
    if (userRole != "Admin")
  {
                return RedirectToAction("Login", "Account");
    }

    var userId = HttpContext.Session.GetString("UserId");
 if (string.IsNullOrEmpty(userId))
  {
           return RedirectToAction("Login", "Account");
            }

       var user = await _context.Users.FindAsync(int.Parse(userId));
         if (user == null)
    {
           return RedirectToAction("Login", "Account");
            }

       return View(user);
     }

        // POST: Admin/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("UserId,Username,Email,Password,Role,CreatedAt")] Models.User user, string? currentPassword, string? newPassword, string? confirmPassword)
    {
            var userRole = HttpContext.Session.GetString("UserRole");
   if (userRole != "Admin")
       {
    return RedirectToAction("Login", "Account");
   }

          var userId = HttpContext.Session.GetString("UserId");
       if (string.IsNullOrEmpty(userId) || int.Parse(userId) != user.UserId)
          {
      return RedirectToAction("Login", "Account");
            }

    // Get current user from database
         var currentUser = await _context.Users.FindAsync(user.UserId);
         if (currentUser == null)
         {
     return RedirectToAction("Login", "Account");
          }

  // Remove password from ModelState if not changing
       ModelState.Remove("Password");
        if (!string.IsNullOrEmpty(newPassword))
      {
                ModelState.Remove("newPassword");
         ModelState.Remove("confirmPassword");
         ModelState.Remove("currentPassword");
      }

         if (ModelState.IsValid)
  {
     try
     {
       // If changing password
  if (!string.IsNullOrEmpty(newPassword))
         {
 // Verify current password
          if (string.IsNullOrEmpty(currentPassword))
         {
          TempData["Error"] = "Current password is required to change your password.";
            return View(user);
 }

      if (currentUser.Password != currentPassword)
   {
         TempData["Error"] = "Current password is incorrect.";
   return View(user);
             }

         if (newPassword != confirmPassword)
         {
           TempData["Error"] = "New password and confirmation password do not match.";
return View(user);
  }

    if (newPassword.Length < 6)
          {
       TempData["Error"] = "Password must be at least 6 characters long.";
      return View(user);
          }

 // Update password
    user.Password = newPassword; // In production, hash this
        }
     else
       {
          // Keep existing password if not changing
          user.Password = currentUser.Password;
           }

     // Update session with new username if changed
             if (user.Username != HttpContext.Session.GetString("Username"))
   {
     HttpContext.Session.SetString("Username", user.Username);
     }

       _context.Update(user);
await _context.SaveChangesAsync();

  TempData["Success"] = "Profile updated successfully!";
           return RedirectToAction(nameof(Profile));
    }
    catch (DbUpdateConcurrencyException)
    {
    if (!UserExists(user.UserId))
 {
       return NotFound();
    }
         else
        {
       throw;
      }
        }
        }

         return View(user);
  }

   private bool UserExists(int id)
        {
  return _context.Users.Any(e => e.UserId == id);
        }
    }
}
