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
                    Address = request.Address,
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

                // 1. Fetch raw data first (Active locations with User details)
                var recentLocations = await _context.LocationHistories
                    .Include(l => l.User)
                    .Where(l => l.Timestamp >= fiveMinutesAgo)
                    .OrderByDescending(l => l.Timestamp)
                    .ToListAsync();

                // 2. Group by User in memory to ensure we get the latest one per user
                var latestLocations = recentLocations
                    .GroupBy(l => l.UserId)
                    .Select(g => g.First()) // Get the top (latest) record for each group
                    .Select(l => new
                    {
                        userId = l.UserId,
                        username = l.User != null ? l.User.Username : "Unknown",
                        email = l.User != null ? l.User.Email : "Unknown",
                        latitude = l.Latitude,
                        longitude = l.Longitude,
                        accuracy = l.Accuracy,
                        address = l.Address,
                        timestamp = l.Timestamp,
                        role = l.User != null ? (l.User.is_Admin ==0 ? "Admin" : "Customer") : "Customer"
                    })
                    .ToList();

                return Json(new { success = true, locations = latestLocations });
            }
            catch (Exception ex)
            {
                // Log the error to your console/logger here if needed
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
                address = l.Address,
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

        // GET: API endpoint to check if location tracking is enabled
        [HttpGet]
        public async Task<IActionResult> CheckTrackingStatus()
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                int userId = int.Parse(userIdString);
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                return Json(new
                {
                    success = true,
                    trackingEnabled = user.LocationTrackingEnabled,
                    enabledDate = user.LocationTrackingEnabledDate
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: API endpoint to get all users location history with filters (admin only)
        [HttpGet]
        public async Task<IActionResult> GetAllUsersLocationHistory(int hours = 24, int? userId = null, string? role = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin")
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                var startTime = DateTime.Now.AddHours(-hours);

                // Build query
                var query = _context.LocationHistories
                    .Include(l => l.User)
                    .Where(l => l.Timestamp >= startTime);

                // Apply user filter
                if (userId.HasValue)
                {
                    query = query.Where(l => l.UserId == userId.Value);
                }

                // Apply role filter
                if (!string.IsNullOrEmpty(role))
                {
                    if (role == "Admin")
                    {
                        query = query.Where(l => l.User.is_Admin == 0);
                    }
                    else if (role == "Customer")
                    {
                        query = query.Where(l => l.User.is_Admin == 1);
                    }
                }

                // Get total count
                var totalRecords = await query.CountAsync();

                // Get paginated results
                var locationHistory = await query
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new
                    {
                        locationId = l.LocationId,
                        userId = l.UserId,
                        username = l.User != null ? l.User.Username : "Unknown",
                        email = l.User != null ? l.User.Email : "Unknown",
                        role = l.User != null ? (l.User.is_Admin == 0 ? "Admin" : "Customer") : "Customer",
                        latitude = l.Latitude,
                        longitude = l.Longitude,
                        accuracy = l.Accuracy,
                        address = l.Address,
                        timestamp = l.Timestamp,
                        deviceInfo = l.DeviceInfo
                    })
                    .ToListAsync();

                // Calculate statistics
                var allRecords = await query.ToListAsync();
                var uniqueUsers = allRecords.Select(l => l.UserId).Distinct().Count();
                var avgAccuracy = allRecords.Any() ? allRecords.Where(l => l.Accuracy.HasValue).Average(l => l.Accuracy ?? 0) : 0;
                var dateRange = allRecords.Any() ? $"{allRecords.Min(l => l.Timestamp):MM/dd} - {allRecords.Max(l => l.Timestamp):MM/dd}" : "-";

                return Json(new
                {
                    success = true,
                    data = locationHistory,
                    totalRecords = totalRecords,
                    totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    currentPage = page,
                    pageSize = pageSize,
                    statistics = new
                    {
                        totalRecords = totalRecords,
                        uniqueUsers = uniqueUsers,
                        avgAccuracy = avgAccuracy,
                        dateRange = dateRange
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: API endpoint to get list of all users for filter dropdown (admin only)
        [HttpGet]
        public async Task<IActionResult> GetAllUsersForFilter()
        {
            try
            {
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin")
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                var users = await _context.Users
                    .Select(u => new
                    {
                        userId = u.UserId,
                        username = u.Username,
                        email = u.Email,
                        role = u.is_Admin == 0 ? "Admin" : "Customer"
                    })
                    .OrderBy(u => u.username)
                    .ToListAsync();

                return Json(new { success = true, users = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
    // Request model for location data
    public class LocationHistoryRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }
        public string? DeviceInfo { get; set; }
        public string? Address { get; set; }
    }


}