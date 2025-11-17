using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;

namespace CalapanCarRentalMVC.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly CarRentalContext _context;

        public UserProfileViewComponent(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userId))
            {
                return View("Default", new
                {
                    Username = "Guest",
                    Role = "Guest",
                    ProfilePicture = (string?)null
                });
            }

            string? profilePicture = null;

            // Get profile picture for customer
            if (userRole == "Customer")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));
                if (user != null)
                {
                    var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == user.Email);

                    if (customer != null)
                    {
                        profilePicture = customer.ProfilePicturePath;
                    }
                }
            }
            // For Admin, you can add admin profile picture logic here if needed
            else if (userRole == "Admin")
            {
                // Future: Add admin profile picture support
                profilePicture = null;
            }

            return View("Default", new
            {
                Username = username ?? "User",
                Role = userRole ?? "User",
                ProfilePicture = profilePicture
            });
        }
    }
}
