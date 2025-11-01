using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Services;

namespace CalapanCarRentalMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly CarRentalContext _context;
        private readonly IEmailService _emailService;

        public AccountController(CarRentalContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                        var carIdValue = TempData["RedirectCarId"];
                        if (carIdValue is int carId)
                        {
                            // Clear the TempData after reading it
                            TempData.Remove("RedirectCarId");
                            TempData.Remove("RedirectMessage");
                            return RedirectToAction("Create", "Rentals", new { carId = carId });
                        }
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

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            return View();
        }

        // POST: Account/SendVerificationCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Email already registered" });
                }

                // Generate 6-digit verification code
                var code = VerificationCodeGenerator.GenerateCode();

                // Delete any existing verification codes for this email
                var existingCodes = _context.EmailVerifications.Where(v => v.Email == email);
                _context.EmailVerifications.RemoveRange(existingCodes);

                // Save verification code to database
                var verification = new EmailVerification
                {
                    Email = email,
                    VerificationCode = code,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(10), // Code expires in 10 minutes
                    IsUsed = false
                };

                _context.EmailVerifications.Add(verification);
                await _context.SaveChangesAsync();

                // Send email
                await _emailService.SendVerificationCodeAsync(email, code);

                return Json(new { success = true, message = "Verification code sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error sending verification code: {ex.Message}" });
            }
        }

        // POST: Account/VerifyCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(string email, string code)
        {
            try
            {
                var verification = await _context.EmailVerifications
                    .Where(v => v.Email == email && v.VerificationCode == code && !v.IsUsed)
                    .OrderByDescending(v => v.CreatedAt)
                    .FirstOrDefaultAsync();

                if (verification == null)
                {
                    return Json(new { success = false, message = "Invalid verification code" });
                }

                if (verification.IsExpired())
                {
                    return Json(new { success = false, message = "Verification code has expired" });
                }

                // Mark code as used
                verification.IsUsed = true;
                _context.Update(verification);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Email verified successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error verifying code: {ex.Message}" });
            }
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/SendPasswordResetCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendPasswordResetCode(string email)
        {
            try
            {
                // Check if user exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return Json(new { success = false, message = "No account found with this email address" });
                }

                // Generate 6-digit reset code
                var code = VerificationCodeGenerator.GenerateCode();

                // Delete any existing password reset codes for this email
                var existingCodes = _context.PasswordResets.Where(pr => pr.Email == email);
                _context.PasswordResets.RemoveRange(existingCodes);

                // Save password reset code to database
                var passwordReset = new PasswordReset
                {
                    Email = email,
                    ResetCode = code,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(10), // Code expires in 10 minutes
                    IsUsed = false
                };

                _context.PasswordResets.Add(passwordReset);
                await _context.SaveChangesAsync();

                // Send email
                await _emailService.SendPasswordResetCodeAsync(email, code);

                return Json(new { success = true, message = "Password reset code sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error sending reset code: {ex.Message}" });
            }
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                // Find password reset record
                var passwordReset = await _context.PasswordResets
                    .Where(pr => pr.Email == email && pr.ResetCode == code && !pr.IsUsed)
                    .OrderByDescending(pr => pr.CreatedAt)
                    .FirstOrDefaultAsync();

                if (passwordReset == null)
                {
                    return Json(new { success = false, message = "Invalid reset code" });
                }

                if (passwordReset.IsExpired())
                {
                    return Json(new { success = false, message = "Reset code has expired. Please request a new one." });
                }

                // Find user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Validate new password
                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                {
                    return Json(new { success = false, message = "Password must be at least 6 characters long" });
                }

                // Update user password
                user.Password = newPassword; // In production, hash this password!
                _context.Update(user);

                // Mark reset code as used
                passwordReset.IsUsed = true;
                _context.Update(passwordReset);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Password reset successfully! Redirecting to login..." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error resetting password: {ex.Message}" });
            }
        }
    }
}