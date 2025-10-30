using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalapanCarRentalMVC.Models
{
    public class Maintenance
    {
 [Key]
    public int MaintenanceId { get; set; }

        [Required]
        public int CarId { get; set; }

   [Required]
    [StringLength(100)]
        [Display(Name = "Maintenance Type")]
 public string MaintenanceType { get; set; } = string.Empty; // Oil Change, Tire Rotation, Brake Service, Engine Repair, etc.

        [Required]
        [StringLength(1000)]
      public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date Scheduled")]
        public DateTime DateScheduled { get; set; }

    [Display(Name = "Date Completed")]
        public DateTime? DateCompleted { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Cost { get; set; }

        [Required]
        [StringLength(50)]
 public string Status { get; set; } = "Scheduled"; // Urgent, Scheduled, Completed

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(200)]
        [Display(Name = "Service Provider")]
        public string? ServiceProvider { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

   public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
     [ForeignKey("CarId")]
      public virtual Car? Car { get; set; }
    }
}
