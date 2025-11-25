using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using CalapanCarRentalMVC.Models;

namespace CalapanCarRentalMVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var from = emailSettings["From"];
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"] ?? "587");
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Calapan Car Rental", from));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Connect to Gmail SMTP server
                await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync(username, password);

                // Send email
                await client.SendAsync(message);

                // Disconnect
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendVerificationCodeAsync(string toEmail, string verificationCode)
        {
            var subject = "Email Verification Code - Calapan Car Rental";
            var body = $@"
   <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
             <div style='text-align: center; margin-bottom: 30px;'>
    <h2 style='color: #dc3545; margin: 0;'> Calapan Car Rental</h2>
     </div>
        
           <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-bottom: 20px;'>
            <h3 style='color: #333; margin-top: 0;'>Email Verification</h3>
    <p style='color: #666; line-height: 1.6;'>
                 Thank you for registering with Calapan Car Rental! To complete your registration, 
       please use the verification code below:
     </p>
  </div>
  
         <div style='background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
      <h1 style='margin: 0; font-size: 36px; letter-spacing: 8px;'>{verificationCode}</h1>
  </div>
      
          <div style='background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;'>
       <p style='margin: 0; color: #856404;'>
           <strong>Important:</strong> This code will expire in 10 minutes.
      </p>
          </div>
       
            <p style='color: #666; line-height: 1.6;'>
          If you did not request this verification code, please ignore this email or contact our support team.
             </p>
       
         <hr style='border: none; border-top: 1px solid #ddd; margin: 30px 0;'>
         
          <div style='text-align: center; color: #999; font-size: 12px;'>
     <p>© 2025 Calapan Car Rental. All rights reserved.</p>
     <p>Calapan City, Oriental Mindoro, Philippines</p>
   <p>
         <a href='mailto:carrentalcalapan@gmail.com' style='color: #dc3545; text-decoration: none;'>carrentalcalapan@gmail.com</a> | 
         Phone: 09053557525 / 09167465112
       </p>
    </div>
            </div>
        ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetCodeAsync(string toEmail, string resetCode)
        {
            var subject = "Password Reset Code - Calapan Car Rental";
            var body = $@"
       <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
         <div style='text-align: center; margin-bottom: 30px;'>
  <h2 style='color: #dc3545; margin: 0;'>?? Calapan Car Rental</h2>
                </div>
     
     <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-bottom: 20px;'>
            <h3 style='color: #333; margin-top: 0;'>Password Reset Request</h3>
               <p style='color: #666; line-height: 1.6;'>
  We received a request to reset your password. Use the code below to reset your password:
      </p>
                </div>
     
            <div style='background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
      <h1 style='margin: 0; font-size: 36px; letter-spacing: 8px;'>{resetCode}</h1>
              </div>
              
        <div style='background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;'>
   <p style='margin: 0; color: #856404;'>
      <strong>?? Important:</strong> This code will expire in 10 minutes.
  </p>
  </div>
   
    <div style='background-color: #f8d7da; padding: 15px; border-left: 4px solid #dc3545; margin: 20px 0;'>
           <p style='margin: 0; color: #721c24;'>
     <strong>Security Notice:</strong> If you did not request a password reset, please ignore this email and your password will remain unchanged. 
       Consider changing your password if you suspect unauthorized access to your account.
            </p>
                </div>
  
   <hr style='border: none; border-top: 1px solid #ddd; margin: 30px 0;'>
     
      <div style='text-align: center; color: #999; font-size: 12px;'>
          <p>© 2025 Calapan Car Rental. All rights reserved.</p>
    <p>Calapan City, Oriental Mindoro, Philippines</p>
    <p>
      <a href='mailto:carrentalcalapan@gmail.com' style='color: #dc3545; text-decoration: none;'>carrentalcalapan@gmail.com</a> | 
     Phone: 09053557525 / 09167465112
           </p>
            </div>
            </div>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendContactMessageAsync(string name, string email, string phone, string message)
        {
            var subject = $"New Contact Message from {name}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <div style='text-align: center; margin-bottom: 30px;'>
                        <h2 style='color: #dc3545; margin: 0;'>Calapan Car Rental</h2>
                        <p style='color: #666; margin-top: 10px;'>New Contact Message</p>
                </div>
    
                 <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-bottom: 20px;'>
                    <h3 style='color: #333; margin-top: 0;'>Contact Information</h3>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='padding: 8px 0; color: #666; font-weight: bold;'>Name:</td>
                            <td style='padding: 8px 0; color: #333;'>{name}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #666; font-weight: bold;'>Email:</td>
                            <td style='padding: 8px 0; color: #333;'><a href='mailto:{email}' style='color: #dc3545; text-decoration: none;'>{email}</a></td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #666; font-weight: bold;'>Phone:</td>
                            <td style='padding: 8px 0; color: #333;'>{(string.IsNullOrEmpty(phone) ? "Not provided" : phone)}</td>
                        </tr>
                    </table>
                </div>
    
<div style='background-color: #fff; padding: 20px; border: 1px solid #ddd; border-radius: 5px; margin-bottom: 20px;'>
        <h3 style='color: #333; margin-top: 0;'>Message</h3>
    <p style='color: #666; line-height: 1.6; white-space: pre-wrap;'>{message}</p>
    </div>
    
    <div style='background-color: #d1ecf1; padding: 15px; border-left: 4px solid #0c5460; margin: 20px 0;'>
 <p style='margin: 0; color: #0c5460;'>
      <strong>Action Required:</strong> Please respond to this inquiry at your earliest convenience.
        </p>
    </div>
    
    <hr style='border: none; border-top: 1px solid #ddd; margin: 30px 0;'>
    
    <div style='text-align: center; color: #999; font-size: 12px;'>
<p>© 2025 Calapan Car Rental. All rights reserved.</p>
        <p>Calapan City, Oriental Mindoro, Philippines</p>
    <p>
       <a href='mailto:carrentalcalapan@gmail.com' style='color: #dc3545; text-decoration: none;'>carrentalcalapan@gmail.com</a> | 
            Phone: 09053557525 / 09167465112
   </p>
    </div>
</div>
            ";

            // Send to your business email
            await SendEmailAsync("carrentalcalapan@gmail.com", subject, body);
        }
    }
}
