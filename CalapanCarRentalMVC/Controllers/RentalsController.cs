using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Filters;

namespace CalapanCarRentalMVC.Controllers
{
    [SessionAuthorization(RequireAuthentication = true)]
    public class RentalsController : Controller
    {
        private readonly CarRentalContext _context;

        public RentalsController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: Rentals
        public async Task<IActionResult> Index(string filterStatus, int? customerId)
        {
            var rentals = _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterStatus))
            {
                rentals = rentals.Where(r => r.Status == filterStatus);
            }

            if (customerId.HasValue)
            {
                rentals = rentals.Where(r => r.CustomerId == customerId.Value);
                ViewBag.CustomerId = customerId.Value;

                // Get customer info for display
                var customer = await _context.Customers.FindAsync(customerId.Value);
                if (customer != null)
                {
                    ViewBag.CustomerName = $"{customer.FirstName} {customer.LastName}";
                }
            }

            return View(await rentals.OrderByDescending(r => r.CreatedAt).ToListAsync());
        }
        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        public async Task<IActionResult> Create(int? carId)
        {
            // Check if user is logged in
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId))
            {
                // Store the car ID in TempData so we can redirect back after login
                if (carId.HasValue)
                {
                    TempData["RedirectCarId"] = carId.Value;
                    TempData["RedirectMessage"] = "Please login or register to rent this car.";
                }
                return RedirectToAction("Login", "Account");
            }

            // Only customers can rent cars
            if (userRole != "Customer")
            {
                TempData["Error"] = "Only customers can rent cars.";
                return RedirectToAction("Index", "Vehicle");
            }

            // Get customer ID from logged in user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
            if (customer == null)
            {
                TempData["Error"] = "Customer profile not found. Please complete your profile.";
                return RedirectToAction("Profile", "Customer");
            }

            // Check if license is expired
            if (customer.LicenseExpiryDate < DateTime.Now)
            {
                TempData["Error"] = "Your driver's license has expired. Please update your license information before booking a car.";
                return RedirectToAction("Profile", "Customer");
            }

            // Set customer ID
            ViewBag.CustomerId = customer.CustomerId;

            ViewData["VehicleId"] = new SelectList(_context.Cars.Where(c => c.Status == "Available"), "VehicleId", "Brand");

            // Pre-select the car if carId is provided
            if (carId.HasValue)
            {
                var selectedCar = await _context.Cars.FindAsync(carId.Value);
                if (selectedCar != null && selectedCar.Status == "Available")
                {
                    ViewBag.SelectedCarId = carId.Value;
                    ViewBag.SelectedCar = selectedCar;
                }
            }

            return View();
        }

        // POST: Rentals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rental rental, IFormFile? governmentIdFile, IFormFile? paymentReceiptFile)
        {
            // Check if user is logged in
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Verify customer's license is not expired
            var customer = await _context.Customers.FindAsync(rental.CustomerId);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Index", "Vehicle");
            }

            if (customer.LicenseExpiryDate < DateTime.Now)
            {
                TempData["Error"] = "Your driver's license has expired. Please update your license information before booking a car.";
                return RedirectToAction("Profile", "Customer");
            }

            // Validate GPS tracking consent
            if (!rental.GpsTrackingConsent)
            {
                ModelState.AddModelError("GpsTrackingConsent", "You must agree to GPS tracking for security and safety purposes to proceed with the rental.");
            }

            // Validate government ID file
            if (governmentIdFile == null || governmentIdFile.Length == 0)
            {
                ModelState.AddModelError("", "Government ID is required");
            }
            else if (governmentIdFile.Length > 5 * 1024 * 1024) // 5MB limit
            {
                ModelState.AddModelError("", "Government ID file size must not exceed 5MB");
            }

            // Validate payment receipt for Gcash and Bank Transfer
            if (rental.PaymentMethod == "Gcash" || rental.PaymentMethod == "Bank Transfer")
            {
                if (paymentReceiptFile == null || paymentReceiptFile.Length == 0)
                {
                    ModelState.AddModelError("", "Payment receipt is required for " + rental.PaymentMethod);
                }
                else if (paymentReceiptFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Payment receipt file size must not exceed 5MB");
                }
            }

            // Remove TotalAmount from ModelState since we calculate it
            ModelState.Remove("TotalAmount");
            // Remove Status from ModelState since we set it
            ModelState.Remove("Status");
            // Remove GovernmentIdPath from ModelState since we set it from the file
            ModelState.Remove("GovernmentIdPath");
            // Remove Car and Customer navigation properties from ModelState
            ModelState.Remove("Car");
            ModelState.Remove("Customer");
            // Remove GpsConsentDate from ModelState since we set it
            ModelState.Remove("GpsConsentDate");

            if (ModelState.IsValid)
            {
                // Validate rental dates
                if (rental.RentalDate < DateTime.Now)
                {
                    ModelState.AddModelError("RentalDate", "Pick-up date cannot be in the past");
                    ViewData["VehicleId"] = new SelectList(_context.Cars.Where(c => c.Status == "Available"), "VehicleId", "Brand", rental.CarId);
                    if (rental.CarId > 0)
                    {
                        var selectedCar1 = await _context.Cars.FindAsync(rental.CarId);
                        ViewBag.SelectedCar = selectedCar1;
                        ViewBag.SelectedCarId = rental.CarId;
                    }
                    ViewBag.CustomerId = rental.CustomerId;
                    return View(rental);
                }

                if (rental.ReturnDate <= rental.RentalDate)
                {
                    ModelState.AddModelError("ReturnDate", "Return date must be after pick-up date");
                    ViewData["VehicleId"] = new SelectList(_context.Cars.Where(c => c.Status == "Available"), "VehicleId", "Brand", rental.CarId);
                    if (rental.CarId > 0)
                    {
                        var selectedCar2 = await _context.Cars.FindAsync(rental.CarId);
                        ViewBag.SelectedCar = selectedCar2;
                        ViewBag.SelectedCarId = rental.CarId;
                    }
                    ViewBag.CustomerId = rental.CustomerId;
                    return View(rental);
                }

                // Upload Government ID
                if (governmentIdFile != null && governmentIdFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "government-ids");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(governmentIdFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await governmentIdFile.CopyToAsync(fileStream);
                    }

                    rental.GovernmentIdPath = "/uploads/government-ids/" + uniqueFileName;
                }

                // Upload Payment Receipt for Gcash and Bank Transfer
                if (paymentReceiptFile != null && paymentReceiptFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "payment-receipts");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(paymentReceiptFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await paymentReceiptFile.CopyToAsync(fileStream);
                    }

                    rental.PaymentReceiptPath = "/uploads/payment-receipts/" + uniqueFileName;
                }

                // Calculate total amount
                var car = await _context.Cars.FindAsync(rental.CarId);
                if (car != null)
                {
                    if (car.Status != "Available")
                    {
                        ModelState.AddModelError("VehicleId", "This car is no longer available");
                        ViewData["VehicleId"] = new SelectList(_context.Cars.Where(c => c.Status == "Available"), "VehicleId", "Brand", rental.CarId);
                        ViewBag.CustomerId = rental.CustomerId;
                        return View(rental);
                    }

                    var hours = (rental.ReturnDate - rental.RentalDate).TotalHours;
                    var days = (int)Math.Ceiling(hours / 24);
                    if (days < 1) days = 1;
                    rental.TotalAmount = car.DailyRate * days;

                    // Car remains available until rental is approved
                    // DON'T update car status yet - wait for admin approval
                }
                else
                {
                    ModelState.AddModelError("VehicleId", "Car not found");
                    ViewData["VehicleId"] = new SelectList(_context.Cars.Where(c => c.Status == "Available"), "VehicleId", "Brand", rental.CarId);
                    ViewBag.CustomerId = rental.CustomerId;
                    return View(rental);
                }

                // Set GPS consent timestamp
                if (rental.GpsTrackingConsent)
                {
                    rental.GpsConsentDate = DateTime.Now;
                }

                rental.Status = "Pending"; // Changed from Active to Pending for approval workflow
                rental.CreatedAt = DateTime.Now;
                _context.Add(rental);
                await _context.SaveChangesAsync();

                // Create notification for ALL admin users
                var adminUsers = await _context.Users.Where(u => u.Role == "Admin").ToListAsync();
                foreach (var adminUser in adminUsers)
                {
                    var adminNotification = new Notification
                    {
                        UserId = adminUser.UserId,
                        Title = "New Rental Request",
                        Message = $"{customer.FirstName} {customer.LastName} requested to rent {car.Brand} {car.Model}. Pick-up: {rental.RentalDate:MMM dd, yyyy}. Total: ₱{rental.TotalAmount:N2}. Please review and approve.",
                        Type = "Info",
                        Icon = "fa-car",
                        ActionUrl = "/Rentals/Index?filterStatus=Pending",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    _context.Notifications.Add(adminNotification);
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Car rental request submitted successfully! Please wait for admin approval.";

                if (userRole == "Customer")
                {
                    return RedirectToAction("MyRentals", "Customer");
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["VehicleId"] = new SelectList(_context.Cars.Where(c => c.Status == "Available"), "VehicleId", "Brand", rental.CarId);
            if (rental.CarId > 0)
            {
                var selectedCar = await _context.Cars.FindAsync(rental.CarId);
                ViewBag.SelectedCar = selectedCar;
                ViewBag.SelectedCarId = rental.CarId;
            }
            ViewBag.CustomerId = rental.CustomerId;
            return View(rental);
        }

        // GET: Rentals/Edit/5
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            ViewData["VehicleId"] = new SelectList(_context.Cars, "VehicleId", "Brand", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", rental.CustomerId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,VehicleId,CustomerId,RentalDate,ReturnDate,Destination,PaymentMethod,TotalAmount,Status,CreatedAt")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();

                    // Update car status if rental is completed
                    if (rental.Status == "Completed")
                    {
                        var car = await _context.Cars.FindAsync(rental.CarId);
                        if (car != null)
                        {
                            car.Status = "Available";
                            _context.Update(car);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
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

            ViewData["VehicleId"] = new SelectList(_context.Cars, "VehicleId", "Brand", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", rental.CustomerId);
            return View(rental);
        }

        // GET: Rentals/Delete/5
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.Include(r => r.Car).FirstOrDefaultAsync(r => r.RentalId == id);
            if (rental != null)
            {
                // Update car status back to available if rental was active
                if (rental.Status == "Active")
                {
                    rental.Car.Status = "Available";
                    _context.Update(rental.Car);
                }

                _context.Rentals.Remove(rental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rentals/Return/5
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Return(int id, DateTime actualReturnDate, decimal? additionalCharges, string? returnNotes)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            rental.ActualReturnDate = DateTime.Now;
            rental.Status = "Completed";

            // Calculate late fee if returned late
            string notificationMessage;
            string notificationType;

            if (rental.ActualReturnDate > rental.ReturnDate)
            {
                var lateDays = (rental.ActualReturnDate.Value - rental.ReturnDate).Days;
                rental.LateFee = lateDays * (rental.Car.DailyRate * 0.2m); // 20% of daily rate as late fee
                rental.TotalAmount += rental.LateFee.Value;
                TempData["Warning"] = $"Late return fee of ₱{rental.LateFee.Value:N2} has been added for {lateDays} day(s) late.";

                notificationMessage = $"Your rental for {rental.Car.Brand} {rental.Car.Model} has been returned {lateDays} day(s) late. A late fee of ₱{rental.LateFee.Value:N2} has been applied. Total amount: ₱{rental.TotalAmount:N2}";
                notificationType = "Warning";
            }
            else
            {
                TempData["Success"] = "Car returned successfully with no late fees!";

                notificationMessage = $"Your rental for {rental.Car.Brand} {rental.Car.Model} has been successfully returned on time. Total amount: ₱{rental.TotalAmount:N2}. Thank you for your business!";
                notificationType = "Success";
            }

            // Update car status to available
            rental.Car.Status = "Available";

            _context.Update(rental);
            _context.Update(rental.Car);

            // Create notification for customer
            var customerUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == rental.Customer.Email);
            if (customerUser != null)
            {
                var notification = new Notification
                {
                    UserId = customerUser.UserId,
                    Title = "Rental Returned",
                    Message = notificationMessage,
                    Type = notificationType,
                    Icon = notificationType == "Success" ? "fa-check-circle" : "fa-exclamation-triangle",
                    ActionUrl = $"/Customer/MyRentals",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Rentals/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Approve(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.Status != "Pending")
            {
                TempData["Error"] = "Only pending rentals can be approved.";
                return RedirectToAction(nameof(Index));
            }

            // Update rental status
            rental.Status = "Active";

            // Update car status
            rental.Car.Status = "Rented";

            _context.Update(rental);
            _context.Update(rental.Car);

            // Create notification for customer
            var customerUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == rental.Customer.Email);
            if (customerUser != null)
            {
                var notification = new Notification
                {
                    UserId = customerUser.UserId,
                    Title = "Rental Approved",
                    Message = $"Your rental request for {rental.Car.Brand} {rental.Car.Model} has been approved! Pick-up date: {rental.RentalDate:MMM dd, yyyy}. Please bring your government ID and payment.",
                    Type = "Success",
                    Icon = "fa-check-circle",
                    ActionUrl = "/Customer/MyRentals",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Rental #{rental.RentalId} has been approved successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Rentals/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorization(Roles = new[] { "Admin" })]
        public async Task<IActionResult> Reject(int id, string? rejectionReason)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.Status != "Pending")
            {
                TempData["Error"] = "Only pending rentals can be rejected.";
                return RedirectToAction(nameof(Index));
            }

            // Update rental status
            rental.Status = "Rejected";

            // Add rejection reason to notes
            if (!string.IsNullOrEmpty(rejectionReason))
            {
                rental.Notes = string.IsNullOrEmpty(rental.Notes)
            ? $"Rejected: {rejectionReason}"
                : $"{rental.Notes}\nRejected: {rejectionReason}";
            }

            // Car remains available (wasn't rented)
            rental.Car.Status = "Available";

            _context.Update(rental);
            _context.Update(rental.Car);

            // Create notification for customer
            var customerUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == rental.Customer.Email);
            if (customerUser != null)
            {
                var notification = new Notification
                {
                    UserId = customerUser.UserId,
                    Title = "Rental Request Rejected",
                    Message = $"Unfortunately, your rental request for {rental.Car.Brand} {rental.Car.Model} has been rejected. " +
                    (string.IsNullOrEmpty(rejectionReason) ? "Please contact us for more information." : $"Reason: {rejectionReason}"),
                    Type = "Danger",
                    Icon = "fa-times-circle",
                    ActionUrl = "/Customer/MyRentals",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Rental #{rental.RentalId} has been rejected.";
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }
    }
}
