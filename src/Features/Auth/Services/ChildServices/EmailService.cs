using System;
using System.Net.Mail;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;

namespace HUBTSOCIAL.src.Features.Auth.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public EmailService()
        {
            // Lấy thông tin từ biến môi trường để bảo mật
            string smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
            int smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            string smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "bachaidang19082004@gmail.com";
            string smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "ffhe zjzf ybfe lovb";

            _fromEmail = smtpUsername; // Đặt email người gửi là email đăng nhập

            // Cấu hình SMTP client
            _smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };
        }

        public async Task<(bool success, string? code)> SendVerificationCodeEmail(string toEmail)
        {
            try
            {
                // Validate email address format
                if (string.IsNullOrWhiteSpace(toEmail) || !IsValidEmail(toEmail))
                {
                    Console.WriteLine("Invalid email address.");
                    return (false, null);
                }

                // Use a more secure random number generator
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    byte[]? randomBytes = new byte[4];
                    rng.GetBytes(randomBytes);
                    string? code = (BitConverter.ToUInt32(randomBytes, 0) % 900000 + 100000).ToString();

                    // Create the email message
                    var mailMessage = new MailMessage(_fromEmail, toEmail)
                    {
                        Subject = "Your verification code",
                        Body = $"Your verification code is: {code}",
                        IsBodyHtml = true
                    };

                    // Send the email
                    await _smtpClient.SendMailAsync(mailMessage);
                    
                    Console.WriteLine($"Verification code sent to {toEmail} successfully.");

                    // Return OTP and success status
                    return (true, code);
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error: {ex.Message}");
                return (false, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return (false, null);
            }
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress? addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
