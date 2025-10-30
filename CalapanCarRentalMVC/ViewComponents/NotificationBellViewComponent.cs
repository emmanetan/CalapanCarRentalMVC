using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;

namespace CalapanCarRentalMVC.ViewComponents
{
 public class NotificationBellViewComponent : ViewComponent
    {
    private readonly CarRentalContext _context;

  public NotificationBellViewComponent(CarRentalContext context)
  {
    _context = context;
      }

    public async Task<IViewComponentResult> InvokeAsync()
   {
      var userId = HttpContext.Session.GetString("UserId");
      
  if (string.IsNullOrEmpty(userId))
      {
     return View("Default", new { UnreadCount = 0, Notifications = new List<object>() });
    }

   var unreadCount = await _context.Notifications
       .CountAsync(n => n.UserId == int.Parse(userId) && !n.IsRead);

    var recentNotifications = await _context.Notifications
  .Where(n => n.UserId == int.Parse(userId))
   .OrderByDescending(n => n.CreatedAt)
 .Take(5)
 .ToListAsync();

  return View("Default", new { UnreadCount = unreadCount, Notifications = recentNotifications });
}
    }
}
