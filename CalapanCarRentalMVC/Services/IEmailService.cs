namespace CalapanCarRentalMVC.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendVerificationCodeAsync(string toEmail, string verificationCode);
        Task SendPasswordResetCodeAsync(string toEmail, string resetCode);
        Task SendContactMessageAsync(string name, string email, string phone, string message);
    }
}
