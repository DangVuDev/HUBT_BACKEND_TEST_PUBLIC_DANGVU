using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;

namespace HUBTSOCIAL.src.Features.Auth.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public VerificationService(UserManager<ApplicationUser> userManager,IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ApplicationUser?> VerifyCodeAsync(ConfirmRequest request)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return null; 
            }

            if (user.VerificationCode != request.OTP || user.VerificationCodeExpires < DateTime.UtcNow)
            {
                return null; 
            }

            user.VerificationCode = null;
            user.VerificationCodeExpires = DateTime.MinValue;
            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return null; 
            }
            return user; 
        }

        public async Task<bool> SendOtpAndUpdateUserAsync(ApplicationUser user)
        {
            // Gửi mã OTP qua email
            if (user.Email is null){
                return false;
            }
            var (success, code) = await _emailService.SendVerificationCodeEmail(user.Email);
            if (!success || string.IsNullOrEmpty(code))
            {
                return false;
            }

            // Cập nhật mã OTP cho người dùng
            bool updated = await UpdateVerificationUserAsync(user, code);

            return updated ? true : false;
        }

        private async Task<bool> UpdateVerificationUserAsync(ApplicationUser user, string code)
        {
            user.VerificationCode = code;
            user.VerificationCodeExpires = DateTime.UtcNow.AddMinutes(5);

            IdentityResult? updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return false;
            }         
            return true;
        }
    }
}
