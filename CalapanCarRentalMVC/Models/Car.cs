using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalapanCarRentalMVC.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TransmissionType { get; set; } = string.Empty; // Manual or Automatic

        [Required]
        public int SeatingCapacity { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Gas Type")]
        public string GasType { get; set; } = "Gasoline"; // Gasoline, Diesel, Electric, Hybrid

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DailyRate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Available"; // Available, Rented, Maintenance

        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
