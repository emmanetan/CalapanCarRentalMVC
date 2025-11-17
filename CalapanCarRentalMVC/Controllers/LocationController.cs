using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;

namespace CalapanCarRentalMVC.Controllers
{
    public class LocationController : Controller
    {
        private readonly CarRentalContext _context;

        public LocationController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: Location/Track - Admin view for tracking
        public IActionResult Track()
        {
            // Check if user is logged in as admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: API endpoint to save location
        [HttpPost]
        public async Task<IActionResult> SaveLocation([FromBody] LocationHistoryRequest request)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                int userId = int.Parse(userIdString);

                var location = new LocationHistory
                {
                    UserId = userId,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Accuracy = request.Accuracy,
                    DeviceInfo = request.DeviceInfo,
                    Timestamp = DateTime.Now
                };

                _context.LocationHistories.Add(location);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Location saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: API endpoint to get latest locations for all users (admin only)
        [HttpGet]
        public async Task<IActionResult> GetLatestLocations()
        {
            try
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin")
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                // Get the latest location for each user from the last 5 minutes
                var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

                var latestLocations = await _context.LocationHistories
                      .Include(l => l.User)
                     .Where(l => l.Timestamp >= fiveMinutesAgo)
                       .GroupBy(l => l.UserId)
                    .Select(g => g.OrderByDescending(l => l.Timestamp).FirstOrDefault())
                     .Select(l => new
                     {
                         userId = l.UserId,
                         username = l.User.Username,
                         email = l.User.Email,
                         latitude = l.Latitude,
                         longitude = l.Longitude,
                         accuracy = l.Accuracy,
                         timestamp = l.Timestamp,
                         role = l.User.Role
                     })
                    .ToListAsync();

                return Json(new { success = true, locations = latestLocations });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: API endpoint to get location history for a specific user (admin only)
        [HttpGet]
        public async Task<IActionResult> GetUserLocationHistory(int userId, int hours = 24)
        {
            try
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin")
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                var startTime = DateTime.Now.AddHours(-hours);

                var locationHistory = await _context.LocationHistories
        .Where(l => l.UserId == userId && l.Timestamp >= startTime)
       .OrderByDescending(l => l.Timestamp)
            .Select(l => new
            {
                latitude = l.Latitude,
                longitude = l.Longitude,
                accuracy = l.Accuracy,
                timestamp = l.Timestamp
            })
                      .ToListAsync();

                return Json(new { success = true, history = locationHistory });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: API endpoint for current user to get their own location
        [HttpGet]
        public IActionResult MyLocation()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }

    // Request model for location data
    public class LocationHistoryRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }
        public string? DeviceInfo { get; set; }
    }
}
