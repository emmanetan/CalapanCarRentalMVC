using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalapanCarRentalMVC.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Danger

        [StringLength(50)]
        public string Icon { get; set; } = "fa-bell"; // Font Awesome icon class

        public bool IsRead { get; set; } = false;

        [StringLength(500)]
        public string? ActionUrl { get; set; } // Optional URL for notification action

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
