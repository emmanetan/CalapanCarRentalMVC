using System.ComponentModel.DataAnnotations;

namespace CalapanCarRentalMVC.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Customer"; // Admin or Customer

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // External login properties
        public string? ExternalLoginProvider { get; set; } // "Google", "Facebook", etc.
        public string? ExternalLoginProviderId { get; set; } // Unique ID from provider

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
