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
        [Display(Name = "Is Admin")]
        public byte is_Admin { get; set; } //0 = Admin,1 = Customer

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // External login properties
        public string? ExternalLoginProvider { get; set; } // "Google", "Facebook", etc.
        public string? ExternalLoginProviderId { get; set; } // Unique ID from provider

        [Display(Name = "Location Tracking Enabled")]
        public bool LocationTrackingEnabled { get; set; } = false;

        public DateTime? LocationTrackingEnabledDate { get; set; }

        [Display(Name = "Verified By Admin")]
        public bool IsVerifiedByAdmin { get; set; } = false;

        public DateTime? VerifiedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
