using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Filters;

namespace CalapanCarRentalMVC.Controllers
{
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public class CustomersController : Controller
    {
        private readonly CarRentalContext _context;

        public CustomersController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string searchString)
        {
            var customers = from c in _context.Customers
                            select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.FirstName.Contains(searchString) ||
               c.LastName.Contains(searchString) ||
           c.Email.Contains(searchString) ||
                   c.PhoneNumber.Contains(searchString));
            }

            return View(await customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
    .Include(c => c.Rentals)
              .ThenInclude(r => r.Car)
       .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Address,LicenseNumber,LicenseExpiryDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FirstName,LastName,Email,PhoneNumber,Address,LicenseNumber,LicenseExpiryDate,CreatedAt")] Customer customer, IFormFile? driverLicenseFile)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing customer to preserve DriverLicensePath
                    var existingCustomer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerId == id);

                    // Handle driver license file upload
                    if (driverLicenseFile != null && driverLicenseFile.Length > 0)
                    {
                        // Validate file size (max 5MB)
                        if (driverLicenseFile.Length > 5 * 1024 * 1024)
                        {
                            TempData["Error"] = "Driver license file size must not exceed 5MB.";
                            return RedirectToAction("Edit", new { id = customer.CustomerId });
                        }

                        // Validate file extension
                        var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
                        var fileExtension = Path.GetExtension(driverLicenseFile.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            TempData["Error"] = "Only PDF, PNG, and JPG files are allowed for driver license.";
                            return RedirectToAction("Edit", new { id = customer.CustomerId });
                        }

                        // Create uploads folder if it doesn't exist
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "licenses");
                        Directory.CreateDirectory(uploadsFolder);

                        // Generate unique filename
                        string uniqueFileName = $"license_{customer.CustomerId}_{Guid.NewGuid()}{fileExtension}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Delete old file if exists
                        if (existingCustomer != null && !string.IsNullOrEmpty(existingCustomer.DriverLicensePath))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCustomer.DriverLicensePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await driverLicenseFile.CopyToAsync(fileStream);
                        }

                        // Update the path
                        customer.DriverLicensePath = $"/uploads/licenses/{uniqueFileName}";
                    }
                    else if (existingCustomer != null)
                    {
                        // Preserve existing path if no new file uploaded
                        customer.DriverLicensePath = existingCustomer.DriverLicensePath;
                    }

                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Customer updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An error occurred: {ex.Message}";
                }
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
            .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
