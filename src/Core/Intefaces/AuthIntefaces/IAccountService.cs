using HUBTSOCIAL.src.Features.Auth.Requests;
using HUBTSOCIAL.src.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;
using HUBTSOCIAL.src.Core.Models;

namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface IAccountService
    {
        Task<(IdentityResult,ApplicationUser)> RegisterAsync(RegisterRequest model);
        Task<(SignInResult,ApplicationUser?)> LoginAsync(LoginRequest model);
        Task<ApplicationUser> FindUserByUserNameAsync(string userName);

        Task<bool> VerifyCodeAsync(ConfirmRequest request);
        Task<bool> SendOtpAndUpdateUserAsync(ApplicationUser user, string email);
    }
}   