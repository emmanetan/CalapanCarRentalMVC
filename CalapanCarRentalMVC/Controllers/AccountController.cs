using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;

namespace CalapanCarRentalMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly CarRentalContext _context;

        public AccountController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // If user is coming from a direct navigation (not from trying to rent a car),
            // and there's no redirect message, clear any stale RedirectCarId
            if (TempData["RedirectMessage"] == null && TempData.ContainsKey("RedirectCarId"))
            {
                TempData.Remove("RedirectCarId");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter email and password";
                return View();
            }

            // Try to find user by email or username
            var user = await _context.Users
        .FirstOrDefaultAsync(u => (u.Email == email || u.Username == email) && u.Password == password);

            if (user != null)
          {
      HttpContext.Session.SetString("UserId", user.UserId.ToString());
    HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("UserRole", user.Role);

       if (user.Role == "Admin")
                {
      return RedirectToAction("Dashboard", "Admin");
            }
        else if (user.Role == "Customer")
        {
     // Check if there's a redirect for renting a car
if (TempData["RedirectCarId"] != null)
              {
          int carId = (int)TempData["RedirectCarId"];
                // Clear the TempData after reading it
            TempData.Remove("RedirectCarId");
     TempData.Remove("RedirectMessage");
         return RedirectToAction("Create", "Rentals", new { carId = carId });
 }
       
            // Default redirect to Customer Dashboard
            return RedirectToAction("Dashboard", "Customer");
        }
  else
 {
             return RedirectToAction("Index", "Home");
 }
            }

       ViewBag.Error = "Invalid email/username or password";
       return View();
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validate AgreeToTerms checkbox
     if (!model.AgreeToTerms)
         {
       ModelState.AddModelError("AgreeToTerms", "You must agree to the Terms of Service and Privacy Policy");
            }

            if (ModelState.IsValid)
            {
                // Generate username from email
      string username = model.Email.Split('@')[0];
      
    // Check if username already exists, append number if needed
     var existingUser = await _context.Users
         .FirstOrDefaultAsync(u => u.Username == username);
          
         int counter = 1;
 string originalUsername = username;
 while (existingUser != null)
           {
         username = $"{originalUsername}{counter}";
        existingUser = await _context.Users
     .FirstOrDefaultAsync(u => u.Username == username);
         counter++;
         }

      // Check if email already exists
  var existingEmail = await _context.Users
          .FirstOrDefaultAsync(u => u.Email == model.Email);

          if (existingEmail != null)
                {
          ModelState.AddModelError("Email", "Email already exists");
return View(model);
    }

       // Create User account
     var user = new User
   {
           Username = username,
 Password = model.Password, // In production, hash this password
           Email = model.Email,
            Role = "Customer",
          CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
    await _context.SaveChangesAsync();

    // Split full name into first and last name
         var nameParts = model.FullName.Trim().Split(' ', 2);
    string firstName = nameParts[0];
      string lastName = nameParts.Length > 1 ? nameParts[1] : "";

     // Create Customer profile with minimal info
    var customer = new Customer
      {
    FirstName = firstName,
    LastName = lastName,
     Email = model.Email,
           PhoneNumber = model.PhoneNumber,
          Address = model.Address,
         LicenseNumber = "", // To be filled in profile
       LicenseExpiryDate = DateTime.Now.AddYears(1), // Default value
      CreatedAt = DateTime.Now
 };

        _context.Customers.Add(customer);
    await _context.SaveChangesAsync();

  TempData["Success"] = "Registration successful! Please login with your credentials.";
    return RedirectToAction(nameof(Login));
 }

    return View(model);
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
