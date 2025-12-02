using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Filters;

namespace CalapanCarRentalMVC.Controllers
{
    public class VehicleController : Controller
    {
        private readonly CarRentalContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VehicleController(CarRentalContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Set layout based on user role
        private void SetLayout()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["Layout"] = userRole == "Admin" ? "_AdminLayout" : "_Layout";
        }

        // GET: Cars
        public async Task<IActionResult> Index(string searchString, string filterStatus)
        {
            SetLayout();
            var cars = from c in _context.Cars
                       select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Brand.Contains(searchString) ||
              c.Model.Contains(searchString) ||
              c.PlateNumber.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(filterStatus))
            {
                cars = cars.Where(c => c.Status == filterStatus);
            }

            return View(await cars.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id, string? returnUrl)
        {
            SetLayout();
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
           .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (car == null)
            {
                return NotFound();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(car);
        }

        // GET: Cars/Create
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public IActionResult Create()
        {
            SetLayout();
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Create([Bind("VehicleId,Brand,Model,Year,Color,PlateNumber,Coding,TransmissionType,SeatingCapacity,GasType,DailyRate,Status,Description")] Car car, IFormFile? imageFile)
        {
            SetLayout();
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cars");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    car.ImageUrl = "/images/cars/" + uniqueFileName;
                }

                car.CreatedAt = DateTime.Now;
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Edit(int? id)
        {
            SetLayout();
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,Brand,Model,Year,Color,PlateNumber,Coding,TransmissionType,SeatingCapacity,GasType,DailyRate,Status,ImageUrl,Description,CreatedAt")] Car car, IFormFile? imageFile)
        {
            SetLayout();
            if (id != car.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cars");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        car.ImageUrl = "/images/cars/" + uniqueFileName;
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.VehicleId))
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
            return View(car);
        }

        // GET: Cars/Delete/5
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Delete(int? id)
        {
            SetLayout();
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                     .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (car == null)
            {
                return NotFound();
            }

            // Pass error message to the view if present
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check for active or pending rentals
            bool hasActiveRental = await _context.Rentals.AnyAsync(r => r.VehicleId == id && (r.Status == "Pending" || r.Status == "Active"));
            if (hasActiveRental)
            {
                TempData["ErrorMessage"] = "Cannot delete this vehicle because it is currently in use for an active or pending rental.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.VehicleId == id);
        }
    }
}
