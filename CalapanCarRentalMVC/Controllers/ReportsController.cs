using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Filters;

namespace CalapanCarRentalMVC.Controllers
{
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public class ReportsController : Controller
    {
        private readonly CarRentalContext _context;

        public ReportsController(CarRentalContext context)
        {
   _context = context;
      }

public async Task<IActionResult> Index()
        {
       // Check if user is Admin
   var userRole = HttpContext.Session.GetString("UserRole");
     if (userRole != "Admin")
     {
 TempData["Error"] = "Access denied. Admin privileges required.";
        return RedirectToAction("Login", "Account");
 }

            // Get summary statistics
var totalRevenue = await _context.Rentals
     .Where(r => r.Status == "Completed")
         .SumAsync(r => r.TotalAmount);

          var totalRentals = await _context.Rentals.CountAsync();
      var activeRentals = await _context.Rentals.CountAsync(r => r.Status == "Active");
          var completedRentals = await _context.Rentals.CountAsync(r => r.Status == "Completed");

 ViewBag.TotalRevenue = totalRevenue;
      ViewBag.TotalRentals = totalRentals;
            ViewBag.ActiveRentals = activeRentals;
            ViewBag.CompletedRentals = completedRentals;

   return View();
        }

     [HttpGet]
        public async Task<IActionResult> GetRevenueData(string period = "daily")
        {
            var today = DateTime.Now.Date;
     List<object> chartData = new List<object>();

            switch (period.ToLower())
            {
                case "daily":
          // Last 30 days
    for (int i = 29; i >= 0; i--)
    {
           var date = today.AddDays(-i);
    var revenue = await _context.Rentals
   .Where(r => r.Status == "Completed" &&
   r.ActualReturnDate.HasValue &&
    r.ActualReturnDate.Value.Date == date)
     .SumAsync(r => r.TotalAmount);

      chartData.Add(new
           {
           date = date.ToString("MMM dd"),
     revenue = revenue,
    count = await _context.Rentals.CountAsync(r => r.Status == "Completed" &&
                 r.ActualReturnDate.HasValue &&
           r.ActualReturnDate.Value.Date == date)
   });
       }
              break;

         case "weekly":
     // Last 12 weeks
          for (int i = 11; i >= 0; i--)
    {
         var weekStart = today.AddDays(-i * 7 - (int)today.DayOfWeek);
          var weekEnd = weekStart.AddDays(6);

    var revenue = await _context.Rentals
              .Where(r => r.Status == "Completed" &&
   r.ActualReturnDate.HasValue &&
                r.ActualReturnDate.Value.Date >= weekStart &&
 r.ActualReturnDate.Value.Date <= weekEnd)
              .SumAsync(r => r.TotalAmount);

            chartData.Add(new
              {
          date = $"{weekStart:MMM dd} - {weekEnd:MMM dd}",
        revenue = revenue,
    count = await _context.Rentals.CountAsync(r => r.Status == "Completed" &&
         r.ActualReturnDate.HasValue &&
   r.ActualReturnDate.Value.Date >= weekStart &&
        r.ActualReturnDate.Value.Date <= weekEnd)
      });
      }
           break;

    case "monthly":
  // Last 12 months
                    for (int i = 11; i >= 0; i--)
    {
var month = today.AddMonths(-i);
             var monthStart = new DateTime(month.Year, month.Month, 1);
   var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var revenue = await _context.Rentals
         .Where(r => r.Status == "Completed" &&
       r.ActualReturnDate.HasValue &&
       r.ActualReturnDate.Value.Date >= monthStart &&
       r.ActualReturnDate.Value.Date <= monthEnd)
        .SumAsync(r => r.TotalAmount);

       chartData.Add(new
      {
               date = monthStart.ToString("MMM yyyy"),
           revenue = revenue,
    count = await _context.Rentals.CountAsync(r => r.Status == "Completed" &&
         r.ActualReturnDate.HasValue &&
      r.ActualReturnDate.Value.Date >= monthStart &&
   r.ActualReturnDate.Value.Date <= monthEnd)
  });
            }
       break;
            }

            return Json(chartData);
        }

        [HttpGet]
        public async Task<IActionResult> GetRentalStatusData()
        {
          var statusData = await _context.Rentals
       .GroupBy(r => r.Status)
          .Select(g => new
             {
      status = g.Key,
         count = g.Count()
    })
           .ToListAsync();

            return Json(statusData);
        }

     [HttpGet]
        public async Task<IActionResult> GetPopularCarsData(string period = "monthly")
  {
  var today = DateTime.Now.Date;
  DateTime startDate;

    switch (period.ToLower())
            {
    case "daily":
        startDate = today.AddDays(-30);
    break;
  case "weekly":
        startDate = today.AddDays(-84); // 12 weeks
             break;
    case "monthly":
        default:
     startDate = today.AddMonths(-12);
          break;
    }

 var popularCars = await _context.Rentals
   .Where(r => r.CreatedAt >= startDate)
        .GroupBy(r => new { r.CarId, r.Car.Brand, r.Car.Model })
       .Select(g => new
      {
   car = $"{g.Key.Brand} {g.Key.Model}",
     count = g.Count(),
        revenue = g.Sum(r => r.TotalAmount)
        })
       .OrderByDescending(x => x.count)
                .Take(10)
        .ToListAsync();

  return Json(popularCars);
        }

        [HttpGet]
    public async Task<IActionResult> GetPaymentMethodData(string period = "monthly")
        {
 var today = DateTime.Now.Date;
            DateTime startDate;

 switch (period.ToLower())
        {
     case "daily":
     startDate = today.AddDays(-30);
            break;
    case "weekly":
    startDate = today.AddDays(-84);
        break;
         case "monthly":
 default:
    startDate = today.AddMonths(-12);
          break;
            }

        var paymentData = await _context.Rentals
                .Where(r => r.CreatedAt >= startDate)
 .GroupBy(r => r.PaymentMethod)
    .Select(g => new
                {
          method = g.Key,
          count = g.Count(),
       revenue = g.Where(r => r.Status == "Completed").Sum(r => r.TotalAmount)
})
     .ToListAsync();

    return Json(paymentData);
  }
    }
}
