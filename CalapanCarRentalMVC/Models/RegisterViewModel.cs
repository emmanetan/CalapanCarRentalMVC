using System.ComponentModel.DataAnnotations;

namespace CalapanCarRentalMVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(200, ErrorMessage = "Full name cannot exceed 200 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "I have read and agree to the Terms of Service and Privacy Policy")]
        public bool AgreeToTerms { get; set; }
    }
}
