using System.ComponentModel.DataAnnotations;

namespace CalapanCarRentalMVC.Models
{
    public class EmailVerification
    {
        [Key]
        public int VerificationId { get; set; }

  [Required]
  [EmailAddress]
        [StringLength(100)]
   public string Email { get; set; } = string.Empty;

 [Required]
        [StringLength(6)]
        public string VerificationCode { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
      public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

    public bool IsExpired()
        {
      return DateTime.Now > ExpiresAt;
     }
    }
}
