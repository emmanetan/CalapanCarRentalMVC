using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;

namespace CalapanCarRentalMVC.Controllers
{
  public class CustomerController : Controller
    {
        private readonly CarRentalContext _context;

        public CustomerController(CarRentalContext context)
        {
            _context = context;
  }

 // Customer Dashboard
        public async Task<IActionResult> Dashboard()
        {
         // Check if user is logged in as customer
     var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
 
            if (userRole != "Customer" || string.IsNullOrEmpty(userId))
       {
                return RedirectToAction("Login", "Account");
       }

    // Get customer data
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
     if (user == null)
{
 return RedirectToAction("Login", "Account");
            }

   var customer = await _context.Customers
    .FirstOrDefaultAsync(c => c.Email == user.Email);

         if (customer != null)
            {
       // Get customer's rental statistics
         ViewBag.CustomerName = $"{customer.FirstName} {customer.LastName}";
              ViewBag.TotalRentals = await _context.Rentals
       .CountAsync(r => r.CustomerId == customer.CustomerId);
            ViewBag.ActiveRentals = await _context.Rentals
            .CountAsync(r => r.CustomerId == customer.CustomerId && r.Status == "Active");
    ViewBag.CompletedRentals = await _context.Rentals
     .CountAsync(r => r.CustomerId == customer.CustomerId && r.Status == "Completed");
        ViewBag.TotalSpent = await _context.Rentals
           .Where(r => r.CustomerId == customer.CustomerId && r.Status == "Completed")
   .SumAsync(r => r.TotalAmount);

 // Get recent rentals
    ViewBag.RecentRentals = await _context.Rentals
        .Include(r => r.Car)
   .Where(r => r.CustomerId == customer.CustomerId)
 .OrderByDescending(r => r.CreatedAt)
              .Take(5)
      .ToListAsync();

  ViewBag.CustomerId = customer.CustomerId;
            }
            else
            {
       ViewBag.CustomerName = user.Username;
 ViewBag.TotalRentals = 0;
                ViewBag.ActiveRentals = 0;
                ViewBag.CompletedRentals = 0;
    ViewBag.TotalSpent = 0;
      ViewBag.RecentRentals = new List<CalapanCarRentalMVC.Models.Rental>();
   }

     return View();
        }

        // My Rentals
  public async Task<IActionResult> MyRentals()
        {
   var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            
  if (userRole != "Customer" || string.IsNullOrEmpty(userId))
    {
          return RedirectToAction("Login", "Account");
            }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
     if (user == null)
        {
    return RedirectToAction("Login", "Account");
     }

  var customer = await _context.Customers
           .FirstOrDefaultAsync(c => c.Email == user.Email);

 if (customer != null)
        {
     var rentals = await _context.Rentals
     .Include(r => r.Car)
   .Where(r => r.CustomerId == customer.CustomerId)
                .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();

  return View(rentals);
  }

    return View(new List<CalapanCarRentalMVC.Models.Rental>());
        }

        // My Profile
        public async Task<IActionResult> Profile()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
      var userId = HttpContext.Session.GetString("UserId");
       
     if (userRole != "Customer" || string.IsNullOrEmpty(userId))
            {
   return RedirectToAction("Login", "Account");
    }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
            if (user == null)
  {
            return RedirectToAction("Login", "Account");
            }

  var customer = await _context.Customers
    .Include(c => c.Rentals)
.FirstOrDefaultAsync(c => c.Email == user.Email);

     if (customer == null)
   {
    return NotFound();
   }

   return View(customer);
        }

        // POST: Customer/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Address,LicenseNumber,LicenseExpiryDate,CreatedAt")] Models.Customer customer)
   {
   var userRole = HttpContext.Session.GetString("UserRole");
   var userId = HttpContext.Session.GetString("UserId");

   if (userRole != "Customer" || string.IsNullOrEmpty(userId))
      {
     return RedirectToAction("Login", "Account");
            }

         var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
     if (user == null)
   {
        return RedirectToAction("Login", "Account");
          }

  var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == user.Email);

          if (existingCustomer == null || existingCustomer.CustomerId != customer.CustomerId)
   {
     TempData["Error"] = "Unauthorized profile update attempt.";
                return RedirectToAction("Profile");
      }

            if (ModelState.IsValid)
     {
      try
     {
     // Update only the allowed fields
  existingCustomer.FirstName = customer.FirstName;
     existingCustomer.LastName = customer.LastName;
   existingCustomer.Email = customer.Email;
   existingCustomer.PhoneNumber = customer.PhoneNumber;
        existingCustomer.Address = customer.Address;
 existingCustomer.LicenseNumber = customer.LicenseNumber;
      existingCustomer.LicenseExpiryDate = customer.LicenseExpiryDate;

          _context.Update(existingCustomer);
   await _context.SaveChangesAsync();

        // Update user email if changed
           if (user.Email != customer.Email)
  {
              user.Email = customer.Email;
       _context.Update(user);
          await _context.SaveChangesAsync();
  }

       TempData["Success"] = "Profile updated successfully!";
                }
     catch (DbUpdateConcurrencyException)
    {
                 TempData["Error"] = "An error occurred while updating your profile. Please try again.";
          }
   catch (Exception)
    {
          TempData["Error"] = "An unexpected error occurred. Please try again.";
       }

      return RedirectToAction("Profile");
  }

            TempData["Error"] = "Please correct the errors and try again.";
   return RedirectToAction("Profile");
        }
    }
}
