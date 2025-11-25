using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Services;
using System.Security.Claims;

namespace CalapanCarRentalMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly CarRentalContext _context;
        private readonly IEmailService _emailService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AccountController(CarRentalContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Account/Login
        public IActionResult Login(bool? cancelled = null, bool? error = null)
        {
            // If user is coming from a direct navigation (not from trying to rent a car),
            // and there's no redirect message, clear any stale RedirectCarId
            if (TempData["RedirectMessage"] == null && TempData.ContainsKey("RedirectCarId"))
            {
                TempData.Remove("RedirectCarId");
            }

            // Handle Google authentication cancellation or error
            if (cancelled == true)
            {
                ViewBag.Warning = "Google sign-in was cancelled. You can try again or login with your email and password.";
            }
            else if (error == true)
            {
                ViewBag.Error = "An error occurred during Google sign-in. Please try again or use traditional login.";
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
                .FirstOrDefaultAsync(u => (u.Email == email || u.Username == email));

            if (user != null && _passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.is_Admin ==0 ? "Admin" : "Customer");

                if (user.is_Admin ==0)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (user.is_Admin ==1)
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

        // POST: External Login (Google)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider)
        {
            // Store redirect car ID in TempData for after login
            var redirectCarId = TempData["RedirectCarId"];
            if (redirectCarId != null)
            {
                TempData.Keep("RedirectCarId");
                TempData.Keep("RedirectMessage");
            }

            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        // GET: Account/ExternalLoginCallback
        public async Task<IActionResult> ExternalLoginCallback(string? remoteError = null)
        {
            // Handle errors from external provider (e.g., user cancelled)
            if (!string.IsNullOrEmpty(remoteError))
            {
                TempData["Error"] = $"Error from Google: {remoteError}";
                return RedirectToAction("Login");
            }

            try
            {
                var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!result.Succeeded || result.Principal == null)
                {
                    TempData["Error"] = "Google sign-in was cancelled or failed. Please try again.";
                    return RedirectToAction("Login");
                }

                var claims = result.Principal.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Unable to retrieve email from Google account.";
                    return RedirectToAction("Login");
                }

                // Check if user already exists with this Google account
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.ExternalLoginProvider == "Google" && u.ExternalLoginProviderId == providerId);

                if (existingUser != null)
                {
                    // User exists, log them in
                    HttpContext.Session.SetString("UserId", existingUser.UserId.ToString());
                    HttpContext.Session.SetString("Username", existingUser.Username);
                    HttpContext.Session.SetString("UserRole", existingUser.is_Admin ==0 ? "Admin" : "Customer");

                    if (existingUser.is_Admin ==0)
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        // Check for car rental redirect
                        if (TempData["RedirectCarId"] != null)
                        {
                            var carIdValue = TempData["RedirectCarId"];
                            if (carIdValue is int carId)
                            {
                                TempData.Remove("RedirectCarId");
                                TempData.Remove("RedirectMessage");
                                return RedirectToAction("Create", "Rentals", new { carId = carId });
                            }
                        }
                        return RedirectToAction("Dashboard", "Customer");
                    }
                }

                // Check if user exists with same email but different provider
                var userWithEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (userWithEmail != null)
                {
                    TempData["Error"] = "An account with this email already exists. Please login with your password or contact support.";
                    return RedirectToAction("Login");
                }

                // New user - create account
                return await CreateGoogleUser(email, name, providerId);
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                TempData["Error"] = "An error occurred during Google sign-in. Please try again or use traditional login.";
                return RedirectToAction("Login");
            }
        }

        private async Task<IActionResult> CreateGoogleUser(string email, string name, string providerId)
        {
            // Generate username from email
            string username = email.Split('@')[0];

            // Check if username already exists, append number if needed
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            int counter = 1;
            string originalUsername = username;
            while (existingUser != null)
            {
                username = $"{originalUsername}{counter}";
                existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                counter++;
            }

            // Create User account (no password needed for external login)
            var user = new User
            {
                Username = username,
                Password = string.Empty, // No password for external login
                Email = email,
                is_Admin =1, // Customer
                ExternalLoginProvider = "Google",
                ExternalLoginProviderId = providerId,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Split full name
            var nameParts = (name ?? username).Trim().Split(' ', 2);
            string firstName = nameParts[0];
            string lastName = nameParts.Length > 1 ? nameParts[1] : "";

            // Create Customer profile
            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = "", // To be filled in profile
                Address = "", // To be filled in profile
                LicenseNumber = "", // To be filled in profile
                LicenseExpiryDate = DateTime.Now.AddYears(1),
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Log in the user
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.is_Admin ==0 ? "Admin" : "Customer");

            TempData["Success"] = "Account created successfully with Google! Please complete your profile.";

            // Check for car rental redirect
            if (TempData["RedirectCarId"] != null)
            {
                var carIdValue = TempData["RedirectCarId"];
                if (carIdValue is int carId)
                {
                    TempData.Remove("RedirectCarId");
                    TempData.Remove("RedirectMessage");
                    return RedirectToAction("Create", "Rentals", new { carId = carId });
                }
            }

            return RedirectToAction("Dashboard", "Customer");
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
                    // Password will be hashed below
                    Email = model.Email,
                    is_Admin =1, // Customer
                    CreatedAt = DateTime.Now
                };
                user.Password = _passwordHasher.HashPassword(user, model.Password);

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
                user.Password = _passwordHasher.HashPassword(user, newPassword);
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