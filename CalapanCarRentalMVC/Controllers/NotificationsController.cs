using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;

namespace CalapanCarRentalMVC.Controllers
{
    public class NotificationsController : Controller
  {
private readonly CarRentalContext _context;

        public NotificationsController(CarRentalContext context)
  {
            _context = context;
        }

        // GET: Notifications/GetUnreadCount
 [HttpGet]
     public async Task<IActionResult> GetUnreadCount()
{
       var userId = HttpContext.Session.GetString("UserId");
    if (string.IsNullOrEmpty(userId))
         {
 return Json(new { count = 0 });
      }

  var count = await _context.Notifications
    .CountAsync(n => n.UserId == int.Parse(userId) && !n.IsRead);

    return Json(new { count });
 }

   // GET: Notifications/GetRecent
   [HttpGet]
        public async Task<IActionResult> GetRecent()
      {
    var userId = HttpContext.Session.GetString("UserId");
   if (string.IsNullOrEmpty(userId))
 {
return Json(new List<object>());
  }

     var notifications = await _context.Notifications
     .Where(n => n.UserId == int.Parse(userId))
.OrderByDescending(n => n.CreatedAt)
      .Take(5)
   .Select(n => new
    {
   n.NotificationId,
  n.Title,
    n.Message,
     n.Type,
      n.Icon,
  n.IsRead,
    n.ActionUrl,
        CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    })
      .ToListAsync();

  return Json(notifications);
 }

// POST: Notifications/MarkAsRead/5
  [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
  {
      var userId = HttpContext.Session.GetString("UserId");
   if (string.IsNullOrEmpty(userId))
       {
    return Json(new { success = false, message = "User not logged in" });
      }

   var notification = await _context.Notifications
     .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == int.Parse(userId));

       if (notification == null)
 {
   return Json(new { success = false, message = "Notification not found" });
      }

       notification.IsRead = true;
            _context.Update(notification);
          await _context.SaveChangesAsync();

      return Json(new { success = true });
 }

        // POST: Notifications/MarkAllAsRead
   [HttpPost]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> MarkAllAsRead()
        {
  var userId = HttpContext.Session.GetString("UserId");
      if (string.IsNullOrEmpty(userId))
   {
       return RedirectToAction("Login", "Account");
    }

      var notifications = await _context.Notifications
       .Where(n => n.UserId == int.Parse(userId) && !n.IsRead)
    .ToListAsync();

foreach (var notification in notifications)
  {
    notification.IsRead = true;
        }

 await _context.SaveChangesAsync();

  TempData["Success"] = $"Marked {notifications.Count} notification(s) as read.";
  return RedirectToAction(nameof(Index));
        }

    // GET: Notifications/Index
   public async Task<IActionResult> Index()
 {
      var userId = HttpContext.Session.GetString("UserId");
     if (string.IsNullOrEmpty(userId))
 {
  return RedirectToAction("Login", "Account");
 }

  var notifications = await _context.Notifications
    .Where(n => n.UserId == int.Parse(userId))
  .OrderByDescending(n => n.CreatedAt)
  .ToListAsync();

      return View(notifications);
 }
  }
}
