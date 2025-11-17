using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalapanCarRentalMVC.Models
{
    public class LocationHistory
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public double? Accuracy { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? DeviceInfo { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
