using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;

namespace HUBTSOCIAL.src.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly RegisterService _registerService;
        private readonly LoginService _loginService;
        private readonly VerificationService _verificationService;
        private readonly UserService _userService;

        public AuthService(RegisterService registerService, LoginService loginService, VerificationService verificationService, UserService userService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _verificationService = verificationService;
            _userService = userService;
        }

        public async Task<(IdentityResult, ApplicationUser)> RegisterAsync(RegisterRequest model)
        {
            return await _registerService.RegisterAsync(model);
        }

        public async Task<(SignInResult, ApplicationUser?)> LoginAsync(LoginRequest model)
        {
            return await _loginService.LoginAsync(model);
        }

        public async Task<ApplicationUser?> VerifyCodeAsync(ConfirmRequest request)
        {
            return await _verificationService.VerifyCodeAsync(request);
        }

        public async Task<bool> ChangeLanguage(ChangeLanguageRequest changeLanguageRequest)
        {
            return await _userService.ChangeLanguage(changeLanguageRequest);
        }

        // Thêm các phương thức khác nếu cần
    }
}
