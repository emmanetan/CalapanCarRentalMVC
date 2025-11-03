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
        public async Task<IActionResult> UpdateProfile([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Address,LicenseNumber,LicenseExpiryDate,CreatedAt")] Models.Customer customer, IFormFile? driverLicenseFile, string? croppedImage)
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
          // Handle profile picture upload (cropped image)
     if (!string.IsNullOrEmpty(croppedImage))
     {
     try
  {
            // Extract base64 data
     var base64Data = croppedImage.Split(',')[1];
               var imageBytes = Convert.FromBase64String(base64Data);

    // Validate file size (max 5MB)
         if (imageBytes.Length > 5 * 1024 * 1024)
       {
            TempData["Error"] = "Profile picture size must not exceed 5MB.";
            return RedirectToAction("Profile");
}

          // Create uploads folder if it doesn't exist
         string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
  Directory.CreateDirectory(uploadsFolder);

 // Generate unique filename
       string uniqueFileName = $"profile_{customer.CustomerId}_{Guid.NewGuid()}.jpg";
string filePath = Path.Combine(uploadsFolder, uniqueFileName);

          // Delete old file if exists
             if (!string.IsNullOrEmpty(existingCustomer.ProfilePicturePath))
            {
     var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCustomer.ProfilePicturePath.TrimStart('/'));
      if (System.IO.File.Exists(oldFilePath))
         {
       System.IO.File.Delete(oldFilePath);
       }
   }

      // Save the file
  await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        // Update the path
              existingCustomer.ProfilePicturePath = $"/uploads/profiles/{uniqueFileName}";
              }
              catch (Exception ex)
    {
           TempData["Error"] = $"Failed to upload profile picture: {ex.Message}";
           return RedirectToAction("Profile");
              }
 }

          // Handle driver license file upload
  if (driverLicenseFile != null && driverLicenseFile.Length > 0)
       {
       // Validate file size (max 5MB)
              if (driverLicenseFile.Length > 5 * 1024 * 1024)
    {
         TempData["Error"] = "Driver license file size must not exceed 5MB.";
        return RedirectToAction("Profile");
      }

 // Validate file extension
         var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
    var fileExtension = Path.GetExtension(driverLicenseFile.FileName).ToLowerInvariant();
         
       if (!allowedExtensions.Contains(fileExtension))
            {
      TempData["Error"] = "Only PDF, PNG, and JPG files are allowed for driver license.";
 return RedirectToAction("Profile");
        }

  // Create uploads folder if it doesn't exist
           string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "licenses");
          Directory.CreateDirectory(uploadsFolder);

        // Generate unique filename
   string uniqueFileName = $"license_{customer.CustomerId}_{Guid.NewGuid()}{fileExtension}";
 string filePath = Path.Combine(uploadsFolder, uniqueFileName);

       // Delete old file if exists
           if (!string.IsNullOrEmpty(existingCustomer.DriverLicensePath))
    {
 var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCustomer.DriverLicensePath.TrimStart('/'));
      if (System.IO.File.Exists(oldFilePath))
     {
  System.IO.File.Delete(oldFilePath);
   }
        }

     // Save the file
     using (var fileStream = new FileStream(filePath, FileMode.Create))
     {
await driverLicenseFile.CopyToAsync(fileStream);
      }

   // Update the path
     existingCustomer.DriverLicensePath = $"/uploads/licenses/{uniqueFileName}";
          }

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
   catch (Exception ex)
    {
          TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
 }

      return RedirectToAction("Profile");
  }

  TempData["Error"] = "Please correct the errors and try again.";
   return RedirectToAction("Profile");
   }

        // POST: Customer/RemoveProfilePicture
  [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfilePicture()
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

    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
            if (customer == null)
            {
         TempData["Error"] = "Customer not found.";
            return RedirectToAction("Profile");
        }

  try
            {
        // Delete the file if exists
                if (!string.IsNullOrEmpty(customer.ProfilePicturePath))
   {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", customer.ProfilePicturePath.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
          {
     System.IO.File.Delete(filePath);
     }

               customer.ProfilePicturePath = null;
        _context.Update(customer);
      await _context.SaveChangesAsync();

       TempData["Success"] = "Profile picture removed successfully!";
    }
        else
                {
   TempData["Error"] = "No profile picture to remove.";
  }
            }
            catch (Exception ex)
            {
      TempData["Error"] = $"Failed to remove profile picture: {ex.Message}";
       }

            return RedirectToAction("Profile");
    }
    }
}
