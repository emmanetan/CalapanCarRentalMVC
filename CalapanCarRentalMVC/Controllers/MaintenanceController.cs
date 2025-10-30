using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;

namespace CalapanCarRentalMVC.Controllers
{
    public class MaintenanceController : Controller
    {
     private readonly CarRentalContext _context;

        public MaintenanceController(CarRentalContext context)
      {
 _context = context;
  }

        // GET: Maintenance
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in as admin
            var userRole = HttpContext.Session.GetString("UserRole");
     if (userRole != "Admin")
  {
   return RedirectToAction("Login", "Account");
            }

      var maintenances = await _context.Maintenances
     .Include(m => m.Car)
      .OrderByDescending(m => m.DateScheduled)
        .ToListAsync();

            // Calculate statistics
     var urgentCount = await _context.Maintenances
    .CountAsync(m => m.Status == "Urgent");

       var scheduledCount = await _context.Maintenances
    .CountAsync(m => m.Status == "Scheduled");

    var completedThisMonth = await _context.Maintenances
    .CountAsync(m => m.Status == "Completed" && 
  m.DateCompleted.HasValue && 
   m.DateCompleted.Value.Month == DateTime.Now.Month &&
         m.DateCompleted.Value.Year == DateTime.Now.Year);

    ViewBag.UrgentCount = urgentCount;
 ViewBag.ScheduledCount = scheduledCount;
 ViewBag.CompletedThisMonth = completedThisMonth;

      return View(maintenances);
   }

        // GET: Maintenance/Details/5
        public async Task<IActionResult> Details(int? id)
 {
            var userRole = HttpContext.Session.GetString("UserRole");
     if (userRole != "Admin")
    {
return RedirectToAction("Login", "Account");
            }

            if (id == null)
  {
       return NotFound();
      }

     var maintenance = await _context.Maintenances
      .Include(m => m.Car)
      .FirstOrDefaultAsync(m => m.MaintenanceId == id);

  if (maintenance == null)
    {
          return NotFound();
            }

  return View(maintenance);
        }

      // GET: Maintenance/Create
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
   return RedirectToAction("Login", "Account");
    }

 ViewBag.Cars = _context.Cars.ToList();
            return View();
        }

        // POST: Maintenance/Create
        [HttpPost]
   [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaintenanceId,CarId,MaintenanceType,Description,DateScheduled,Cost,Status,Notes,ServiceProvider")] Maintenance maintenance)
     {
    var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
      {
         return RedirectToAction("Login", "Account");
   }

            if (ModelState.IsValid)
         {
       maintenance.CreatedAt = DateTime.Now;
          maintenance.UpdatedAt = DateTime.Now;
_context.Add(maintenance);

      // Update car status if maintenance is urgent or scheduled
    if (maintenance.Status == "Urgent" || maintenance.Status == "Scheduled")
              {
        var car = await _context.Cars.FindAsync(maintenance.CarId);
      if (car != null && car.Status == "Available")
              {
                car.Status = "Maintenance";
          _context.Update(car);
    }
       }

                await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
  }

ViewBag.Cars = _context.Cars.ToList();
            return View(maintenance);
        }

     // GET: Maintenance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
    var userRole = HttpContext.Session.GetString("UserRole");
    if (userRole != "Admin")
     {
 return RedirectToAction("Login", "Account");
        }

            if (id == null)
            {
  return NotFound();
            }

            var maintenance = await _context.Maintenances.FindAsync(id);
    if (maintenance == null)
            {
          return NotFound();
          }

     ViewBag.Cars = _context.Cars.ToList();
    return View(maintenance);
 }

        // POST: Maintenance/Edit/5
        [HttpPost]
 [ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("MaintenanceId,CarId,MaintenanceType,Description,DateScheduled,DateCompleted,Cost,Status,Notes,ServiceProvider,CreatedAt")] Maintenance maintenance)
 {
var userRole = HttpContext.Session.GetString("UserRole");
  if (userRole != "Admin")
      {
    return RedirectToAction("Login", "Account");
   }

  if (id != maintenance.MaintenanceId)
{
    return NotFound();
            }

            if (ModelState.IsValid)
    {
            try
    {
     maintenance.UpdatedAt = DateTime.Now;

         // If status changed to Completed, set DateCompleted
         if (maintenance.Status == "Completed" && !maintenance.DateCompleted.HasValue)
          {
   maintenance.DateCompleted = DateTime.Now;
    }

       _context.Update(maintenance);

    // Update car status based on maintenance status
 var car = await _context.Cars.FindAsync(maintenance.CarId);
    if (car != null)
     {
   if (maintenance.Status == "Completed")
  {
// Check if there are other active maintenances for this car
        var hasActiveMaintenance = await _context.Maintenances
       .AnyAsync(m => m.CarId == maintenance.CarId && 
       (m.Status == "Urgent" || m.Status == "Scheduled") && 
      m.MaintenanceId != maintenance.MaintenanceId);

     if (!hasActiveMaintenance && car.Status == "Maintenance")
    {
          car.Status = "Available";
 _context.Update(car);
   }
}
     else if ((maintenance.Status == "Urgent" || maintenance.Status == "Scheduled") && car.Status == "Available")
       {
       car.Status = "Maintenance";
           _context.Update(car);
      }
 }

    await _context.SaveChangesAsync();
   }
   catch (DbUpdateConcurrencyException)
       {
     if (!MaintenanceExists(maintenance.MaintenanceId))
   {
   return NotFound();
       }
       else
        {
    throw;
    }
    }
           return RedirectToAction(nameof(Index));
    }

          ViewBag.Cars = _context.Cars.ToList();
  return View(maintenance);
        }

        // GET: Maintenance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
   return RedirectToAction("Login", "Account");
            }

            if (id == null)
    {
             return NotFound();
   }

            var maintenance = await _context.Maintenances
             .Include(m => m.Car)
      .FirstOrDefaultAsync(m => m.MaintenanceId == id);

        if (maintenance == null)
            {
   return NotFound();
            }

            return View(maintenance);
        }

   // POST: Maintenance/Delete/5
        [HttpPost, ActionName("Delete")]
     [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
    {
  var userRole = HttpContext.Session.GetString("UserRole");
      if (userRole != "Admin")
     {
   return RedirectToAction("Login", "Account");
    }

var maintenance = await _context.Maintenances.FindAsync(id);
     if (maintenance != null)
  {
          var carId = maintenance.CarId;
_context.Maintenances.Remove(maintenance);
     await _context.SaveChangesAsync();

 // Check if there are other active maintenances for this car
       var hasActiveMaintenance = await _context.Maintenances
     .AnyAsync(m => m.CarId == carId && 
     (m.Status == "Urgent" || m.Status == "Scheduled"));

  // If no active maintenance, set car back to available
     if (!hasActiveMaintenance)
       {
  var car = await _context.Cars.FindAsync(carId);
     if (car != null && car.Status == "Maintenance")
         {
     car.Status = "Available";
   _context.Update(car);
 await _context.SaveChangesAsync();
     }
 }
   }

  return RedirectToAction(nameof(Index));
     }

    // POST: Maintenance/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteMaintenance(int id, decimal? actualCost, string? completionNotes)
    {
   var userRole = HttpContext.Session.GetString("UserRole");
       if (userRole != "Admin")
       {
  return RedirectToAction("Login", "Account");
   }

 var maintenance = await _context.Maintenances
      .Include(m => m.Car)
  .FirstOrDefaultAsync(m => m.MaintenanceId == id);

  if (maintenance == null)
  {
         return NotFound();
         }

      // Update maintenance to completed
     maintenance.Status = "Completed";
      maintenance.DateCompleted = DateTime.Now;
     maintenance.UpdatedAt = DateTime.Now;

  // Update cost if provided
        if (actualCost.HasValue && actualCost.Value > 0)
      {
     maintenance.Cost = actualCost.Value;
         }

    // Add completion notes if provided
     if (!string.IsNullOrEmpty(completionNotes))
          {
 maintenance.Notes = string.IsNullOrEmpty(maintenance.Notes) 
   ? $"Completed: {completionNotes}" 
: $"{maintenance.Notes}\nCompleted: {completionNotes}";
   }

    _context.Update(maintenance);

      // Update car status back to available
  if (maintenance.Car != null)
         {
       // Check if there are other active maintenances for this car
        var hasActiveMaintenance = await _context.Maintenances
   .AnyAsync(m => m.CarId == maintenance.CarId && 
       (m.Status == "Urgent" || m.Status == "Scheduled") && 
         m.MaintenanceId != maintenance.MaintenanceId);

if (!hasActiveMaintenance && maintenance.Car.Status == "Maintenance")
     {
   maintenance.Car.Status = "Available";
      _context.Update(maintenance.Car);
          
        // Notify customers who have active rentals for this car type (future bookings)
         // For now, notify all customers via admin notification or create system notification
     }
   }

            await _context.SaveChangesAsync();

   TempData["Success"] = $"Maintenance for {maintenance.Car?.Brand} {maintenance.Car?.Model} has been marked as completed!";
     return RedirectToAction(nameof(Index));
   }

private bool MaintenanceExists(int id)
   {
         return _context.Maintenances.Any(e => e.MaintenanceId == id);
   }
    }
}
