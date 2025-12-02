using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalapanCarRentalMVC.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Pick-up Date & Time")]
        public DateTime RentalDate { get; set; }

        [Required]
        [Display(Name = "Return Date & Time")]
        public DateTime ReturnDate { get; set; }

        public DateTime? ActualReturnDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")] 
        public decimal? LateFee { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Security Deposit")]
        public decimal SecurityDeposit { get; set; } = 2000;

        [Display(Name = "Deposit Status")]
        [StringLength(50)]
        public string? DepositStatus { get; set; } // null = Not Returned, "Refunded" = Returned, "Forfeited" = Kept due to damage

        public DateTime? DepositReturnDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Active, Completed, Cancelled

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Destination (within Oriental Mindoro)")]
        public string Destination { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Government ID")]
        public string? GovernmentIdPath { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Gcash, Bank Transfer

        [StringLength(500)]
        [Display(Name = "Payment Receipt")]
        public string? PaymentReceiptPath { get; set; }

        [Required]
        [Display(Name = "GPS Tracking Consent")]
        public bool GpsTrackingConsent { get; set; } = false;

        public DateTime? GpsConsentDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("VehicleId")]
        public virtual Car Car { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }
}
