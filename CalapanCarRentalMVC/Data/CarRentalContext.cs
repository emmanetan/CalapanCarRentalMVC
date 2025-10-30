using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Models;

namespace CalapanCarRentalMVC.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Rental>()
         .HasOne(r => r.Car)
         .WithMany(c => c.Rentals)
    .HasForeignKey(r => r.CarId)
      .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
           .HasOne(r => r.Customer)
      .WithMany(c => c.Rentals)
           .HasForeignKey(r => r.CustomerId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
   .HasOne(m => m.Car)
                .WithMany()
    .HasForeignKey(m => m.CarId)
    .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            modelBuilder.Entity<User>().HasData(
                  new User
                  {
                      UserId = 1,
                      Username = "admin",
                      Password = "admin123", // In production, use hashed passwords
                      Role = "Admin",
                      Email = "admin@calapancarrental.com",
                      CreatedAt = DateTime.Now
                  }
               );

            // Seed sample cars
            modelBuilder.Entity<Car>().HasData(
        new Car
        {
            CarId = 1,
            Brand = "Toyota",
            Model = "Vios",
            Year = 2023,
            Color = "White",
            PlateNumber = "ABC1234",
            TransmissionType = "Automatic",
            SeatingCapacity = 5,
            GasType = "Gasoline",
            DailyRate = 1500.00m,
            Status = "Available",
            Description = "Fuel-efficient and reliable sedan perfect for city driving.",
            ImageUrl = "/images/toyota-vios.jpg",
            CreatedAt = DateTime.Now
        },
                new Car
                {
                    CarId = 2,
                    Brand = "Honda",
                    Model = "Civic",
                    Year = 2023,
                    Color = "Black",
                    PlateNumber = "DEF5678",
                    TransmissionType = "Automatic",
                    SeatingCapacity = 5,
                    GasType = "Gasoline",
                    DailyRate = 2000.00m,
                    Status = "Available",
                    Description = "Sporty sedan with advanced features and comfort.",
                    ImageUrl = "/images/honda-civic.jpg",
                    CreatedAt = DateTime.Now
                },
          new Car
          {
              CarId = 3,
              Brand = "Mitsubishi",
              Model = "Montero Sport",
              Year = 2023,
              Color = "Silver",
              PlateNumber = "GHI9012",
              TransmissionType = "Automatic",
              SeatingCapacity = 7,
              GasType = "Diesel",
              DailyRate = 3000.00m,
              Status = "Available",
              Description = "Spacious SUV perfect for family trips and adventures.",
              ImageUrl = "/images/montero-sport.jpg",
              CreatedAt = DateTime.Now
          }
        );
        }
    }
}
