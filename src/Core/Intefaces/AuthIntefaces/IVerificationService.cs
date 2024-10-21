using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;

namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface IVerificationService
    {
        Task<ApplicationUser?> VerifyCodeAsync(ConfirmRequest request);
        Task<bool> SendOtpAndUpdateUserAsync(ApplicationUser user);
    }
}