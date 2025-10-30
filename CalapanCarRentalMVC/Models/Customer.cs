using System.ComponentModel.DataAnnotations;

namespace CalapanCarRentalMVC.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        public DateTime LicenseExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
