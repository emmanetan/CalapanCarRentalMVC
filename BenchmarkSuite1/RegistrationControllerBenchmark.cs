using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Models;
using CalapanCarRentalMVC.Services;
using CalapanCarRentalMVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VSDiagnostics;

namespace CalapanCarRentalMVC.Benchmarks
{
    [Microsoft.VSDiagnostics.Diagnosers.ConcurrencyVisualizerProfiler]
    [CPUUsageDiagnoser]
    public class RegistrationControllerBenchmark
    {
        private CarRentalContext _context = null !;
        private RegistrationController _controller = null !;
        private IEmailService _emailService = null !;
        private List<int> _testUserIds = new List<int>();
        [GlobalSetup]
        public void Setup()
        {
            // Setup in-memory database for benchmarking
            var options = new DbContextOptionsBuilder<CarRentalContext>().UseInMemoryDatabase(databaseName: "RegistrationBenchmarkDb_" + Guid.NewGuid()).Options;
            _context = new CarRentalContext(options);
            // Create mock email service
            _emailService = new MockEmailService();
            // Seed test data
            SeedTestData();
            _controller = new RegistrationController(_context, _emailService);
        }

        private void SeedTestData()
        {
            // Create 100 pending users for realistic testing
            for (int i = 1; i <= 100; i++)
            {
                var user = new User
                {
                    UserId = i,
                    Username = $"testuser{i}",
                    Password = "password123",
                    Email = $"testuser{i}@test.com",
                    is_Admin = 1, // Customer
                    IsVerifiedByAdmin = false,
                    CreatedAt = DateTime.Now.AddDays(-i)
                };
                _context.Users.Add(user);
                // Add customer for half the users
                if (i % 2 == 0)
                {
                    var customer = new Customer
                    {
                        CustomerId = i,
                        FirstName = $"First{i}",
                        LastName = $"Last{i}",
                        Email = $"testuser{i}@test.com",
                        PhoneNumber = $"123456789{i}",
                        Address = $"Address {i}",
                        LicenseNumber = $"LIC{i}",
                        LicenseCode = $"A{i}",
                        LicenseExpiryDate = DateTime.Now.AddYears(1)
                    };
                    _context.Customers.Add(customer);
                }

                _testUserIds.Add(i);
            }

            // Create some verified users
            for (int i = 101; i <= 150; i++)
            {
                var user = new User
                {
                    UserId = i,
                    Username = $"verified{i}",
                    Password = "password123",
                    Email = $"verified{i}@test.com",
                    is_Admin = 1,
                    IsVerifiedByAdmin = true,
                    VerifiedDate = DateTime.Now.AddDays(-10),
                    CreatedAt = DateTime.Now.AddDays(-i)
                };
                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }

        [Benchmark]
        public async Task<IActionResult> Index_LoadPendingRegistrations()
        {
            return await _controller.Index();
        }

        [Benchmark]
        public async Task<IActionResult> Details_LoadUserWithCustomer()
        {
            // Test with a user that has a customer record
            return await _controller.Details(2);
        }

        [Benchmark]
        public async Task<IActionResult> Approve_UserRegistration()
        {
            // Find an unapproved user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.is_Admin == 1 && !u.IsVerifiedByAdmin);
            if (user != null)
            {
                var result = await _controller.Approve(user.UserId);
                // Reset state for next iteration
                user.IsVerifiedByAdmin = false;
                user.VerifiedDate = null;
                // Remove notification created by approve
                var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == user.UserId);
                if (notification != null)
                {
                    _context.Notifications.Remove(notification);
                }

                await _context.SaveChangesAsync();
                return result;
            }

            return new NotFoundResult();
        }

        [Benchmark]
        public async Task<IActionResult> VerifiedUsers_LoadList()
        {
            return await _controller.VerifiedUsers();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }
    }

    // Mock email service for benchmarking
    public class MockEmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            return Task.CompletedTask;
        }

        public Task SendVerificationCodeAsync(string toEmail, string verificationCode)
        {
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(string toEmail, string resetCode)
        {
            return Task.CompletedTask;
        }

        public Task SendContactMessageAsync(string name, string email, string phone, string message)
        {
            return Task.CompletedTask;
        }

        public Task SendRegistrationApprovedAsync(string toEmail, string username)
        {
            return Task.CompletedTask;
        }

        public Task SendRegistrationRejectedAsync(string toEmail, string username)
        {
            return Task.CompletedTask;
        }

        public Task SendRegistrationPendingAsync(string toEmail, string username)
        {
            return Task.CompletedTask;
        }
    }
}