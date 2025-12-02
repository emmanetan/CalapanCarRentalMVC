namespace CalapanCarRentalMVC.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendVerificationCodeAsync(string toEmail, string verificationCode);
        Task SendPasswordResetCodeAsync(string toEmail, string resetCode);
        Task SendContactMessageAsync(string name, string email, string phone, string message);
        Task SendRegistrationApprovedAsync(string toEmail, string username);
        Task SendRegistrationRejectedAsync(string toEmail, string username);
        Task SendRegistrationPendingAsync(string toEmail, string username);
        Task SendRentalApprovedAsync(string toEmail, string customerName, string carDetails, DateTime pickupDate, DateTime returnDate, decimal totalAmount, decimal rentalCost, decimal securityDeposit);
    }
}
