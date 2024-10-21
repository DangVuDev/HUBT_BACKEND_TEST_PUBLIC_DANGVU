using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Features.Auth.Requests;
using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;

namespace HUBTSOCIAL.src.Features.Auth.Services
{
    public class LoginService : ILoginService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<(SignInResult,ApplicationUser?)> LoginAsync(LoginRequest model)
        {
            SignInResult? result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);           
            return result.Succeeded 
                ? (result,await _userManager.FindByNameAsync(model.UserName)) 
                : (result,null);
        }
    }
}
