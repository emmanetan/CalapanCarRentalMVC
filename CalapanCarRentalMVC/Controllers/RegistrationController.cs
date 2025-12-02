using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Filters;
using CalapanCarRentalMVC.Services;

namespace CalapanCarRentalMVC.Controllers
{
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public class RegistrationController : Controller
    {
        private readonly CarRentalContext _context;
        private readonly IEmailService _emailService;

        public RegistrationController(CarRentalContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Registration/Index
        public async Task<IActionResult> Index()
        {
            // Get all users who are customers and not verified by admin
            var pendingRegistrations = await _context.Users
                .Where(u => u.is_Admin == 1 && !u.IsVerifiedByAdmin)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(pendingRegistrations);
        }

        // GET: Registration/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
               .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            // Get the associated customer information
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == user.Email);

            ViewBag.Customer = customer;

            return View(user);
        }

        // POST: Registration/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsVerifiedByAdmin = true;
            user.VerifiedDate = DateTime.Now;

            _context.Update(user);
            await _context.SaveChangesAsync();

            // Create a notification for the user
            var notification = new Notification
            {
                UserId = user.UserId,
                Message = "Your account has been verified by the administrator. You can now log in to the system.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send approval email
            try
            {
                await _emailService.SendRegistrationApprovedAsync(user.Email, user.Username);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the approval process
                // The user is still approved even if email fails
                TempData["Warning"] = $"User {user.Username} has been approved, but the email notification could not be sent.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = $"User {user.Username} has been approved successfully. Confirmation email sent.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Registration/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Store user info before deletion for email notification
            var userEmail = user.Email;
            var username = user.Username;

            // Find and delete associated customer record
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == user.Email);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            // Create a notification before deleting the user
            var notification = new Notification
            {
                UserId = user.UserId,
                Message = "Your registration has been rejected by the administrator. Please contact support for more information.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send rejection email before deleting the user
            try
            {
                await _emailService.SendRegistrationRejectedAsync(userEmail, username);
            }
            catch (Exception ex)
            {
                // Log the error but continue with deletion
                // Email failure shouldn't stop the rejection process
            }

            // Delete the user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Registration for {username} has been rejected and removed. Notification email sent.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Registration/VerifiedUsers
        public async Task<IActionResult> VerifiedUsers()
        {
            // Get all users who are customers and verified by admin
            var verifiedUsers = await _context.Users
                .Where(u => u.is_Admin == 1 && u.IsVerifiedByAdmin)
                .OrderByDescending(u => u.VerifiedDate)
                .ToListAsync();

            return View(verifiedUsers);
        }
    }
}
