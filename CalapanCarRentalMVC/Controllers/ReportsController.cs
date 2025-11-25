using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Filters;
using ClosedXML.Excel;

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
            var totalCustomers = await _context.Customers.CountAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalRentals = totalRentals;
            ViewBag.ActiveRentals = activeRentals;
            ViewBag.CompletedRentals = completedRentals;
            ViewBag.TotalCustomers = totalCustomers;

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
                   .GroupBy(r => new { CarId = r.VehicleId, r.Car.Brand, r.Car.Model })
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
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                // Create a new workbook
                using var workbook = new XLWorkbook();

                // Summary Sheet
                var summarySheet = workbook.Worksheets.Add("Summary");
                summarySheet.Cell("A1").Value = "Calapan Car Rental - Reports Summary";
                summarySheet.Cell("A1").Style.Font.Bold = true;
                summarySheet.Cell("A1").Style.Font.FontSize = 16;
                summarySheet.Range("A1:B1").Merge();

                summarySheet.Cell("A2").Value = "Generated:";
                summarySheet.Cell("B2").Value = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");

                // Summary Statistics
                var totalRevenue = await _context.Rentals
                .Where(r => r.Status == "Completed")
                     .SumAsync(r => r.TotalAmount);
                var totalRentals = await _context.Rentals.CountAsync();
                var activeRentals = await _context.Rentals.CountAsync(r => r.Status == "Active");
                var completedRentals = await _context.Rentals.CountAsync(r => r.Status == "Completed");
                var totalCustomers = await _context.Customers.CountAsync();

                summarySheet.Cell("A4").Value = "Metric";
                summarySheet.Cell("B4").Value = "Value";
                summarySheet.Cell("A4").Style.Font.Bold = true;
                summarySheet.Cell("B4").Style.Font.Bold = true;

                summarySheet.Cell("A5").Value = "Total Revenue";
                summarySheet.Cell("B5").Value = totalRevenue;
                summarySheet.Cell("B5").Style.NumberFormat.Format = "₱#,##0.00";

                summarySheet.Cell("A6").Value = "Total Rentals";
                summarySheet.Cell("B6").Value = totalRentals;

                summarySheet.Cell("A7").Value = "Active Rentals";
                summarySheet.Cell("B7").Value = activeRentals;

                summarySheet.Cell("A8").Value = "Completed Rentals";
                summarySheet.Cell("B8").Value = completedRentals;

                summarySheet.Cell("A9").Value = "Total Customers Registered";
                summarySheet.Cell("B9").Value = totalCustomers;

                summarySheet.Columns().AdjustToContents();

                // Revenue Trends Sheet (Monthly)
                var revenueSheet = workbook.Worksheets.Add("Revenue Trends");
                revenueSheet.Cell("A1").Value = "Revenue Trends (Last 12 Months)";
                revenueSheet.Cell("A1").Style.Font.Bold = true;
                revenueSheet.Cell("A1").Style.Font.FontSize = 14;

                revenueSheet.Cell("A2").Value = "Period";
                revenueSheet.Cell("B2").Value = "Revenue";
                revenueSheet.Cell("C2").Value = "Number of Rentals";
                revenueSheet.Range("A2:C2").Style.Font.Bold = true;

                var today = DateTime.Now.Date;
                int row = 3;
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

                    var count = await _context.Rentals.CountAsync(r => r.Status == "Completed" &&
                 r.ActualReturnDate.HasValue &&
           r.ActualReturnDate.Value.Date >= monthStart &&
                    r.ActualReturnDate.Value.Date <= monthEnd);

                    revenueSheet.Cell($"A{row}").Value = monthStart.ToString("MMM yyyy");
                    revenueSheet.Cell($"B{row}").Value = revenue;
                    revenueSheet.Cell($"B{row}").Style.NumberFormat.Format = "₱#,##0.00";
                    revenueSheet.Cell($"C{row}").Value = count;
                    row++;
                }

                revenueSheet.Columns().AdjustToContents();

                // Rental Status Distribution Sheet
                var statusSheet = workbook.Worksheets.Add("Rental Status");
                statusSheet.Cell("A1").Value = "Rental Status Distribution";
                statusSheet.Cell("A1").Style.Font.Bold = true;
                statusSheet.Cell("A1").Style.Font.FontSize = 14;

                statusSheet.Cell("A2").Value = "Status";
                statusSheet.Cell("B2").Value = "Count";
                statusSheet.Range("A2:B2").Style.Font.Bold = true;

                var statusData = await _context.Rentals
                       .GroupBy(r => r.Status)
                 .Select(g => new
                 {
                     Status = g.Key,
                     Count = g.Count()
                 })
                  .ToListAsync();

                row = 3;
                foreach (var status in statusData)
                {
                    statusSheet.Cell($"A{row}").Value = status.Status;
                    statusSheet.Cell($"B{row}").Value = status.Count;
                    row++;
                }

                statusSheet.Columns().AdjustToContents();

                // Popular Cars Sheet
                var carsSheet = workbook.Worksheets.Add("Popular Cars");
                carsSheet.Cell("A1").Value = "Most Rented Vehicles (Last 12 Months)";
                carsSheet.Cell("A1").Style.Font.Bold = true;
                carsSheet.Cell("A1").Style.Font.FontSize = 14;

                carsSheet.Cell("A2").Value = "Vehicle";
                carsSheet.Cell("B2").Value = "Rental Count";
                carsSheet.Cell("C2").Value = "Total Revenue";
                carsSheet.Range("A2:C2").Style.Font.Bold = true;

                var startDate = today.AddMonths(-12);
                var popularCars = await _context.Rentals
    .Where(r => r.CreatedAt >= startDate)
             .GroupBy(r => new { CarId = r.VehicleId, r.Car.Brand, r.Car.Model })
            .Select(g => new
            {
                Car = $"{g.Key.Brand} {g.Key.Model}",
                Count = g.Count(),
                Revenue = g.Sum(r => r.TotalAmount)
            })
        .OrderByDescending(x => x.Count)
         .Take(10)
            .ToListAsync();

                row = 3;
                foreach (var car in popularCars)
                {
                    carsSheet.Cell($"A{row}").Value = car.Car;
                    carsSheet.Cell($"B{row}").Value = car.Count;
                    carsSheet.Cell($"C{row}").Value = car.Revenue;
                    carsSheet.Cell($"C{row}").Style.NumberFormat.Format = "₱#,##0.00";
                    row++;
                }

                carsSheet.Columns().AdjustToContents();

                // Generate the Excel file
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"CarRentalReports_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream.ToArray(),
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                      fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating Excel report: {ex.Message}";
                return RedirectToAction("Index");
            }
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
