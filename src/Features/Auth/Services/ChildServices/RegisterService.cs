using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AspNetCore.Identity.MongoDbCore.Models;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;

namespace HUBTSOCIAL.src.Features.Auth.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<MongoIdentityRole<Guid>> _roleManager;

        public RegisterService(UserManager<ApplicationUser> userManager, RoleManager<MongoIdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(IdentityResult,ApplicationUser)> RegisterAsync(RegisterRequest model)
        {
            // Kiểm tra null cho model
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model không thể null.");
            }
            ApplicationUser? accountAlreadyExists = await _userManager.FindByNameAsync(model.UserName);

            if (accountAlreadyExists is null)
            {
                throw new Exception("Tài khoản đã được đăng ký.");
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

       
            IdentityResult? result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                const string defaultRole = "USER";

             
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new MongoIdentityRole<Guid>(defaultRole)); // Sử dụng MongoIdentityRole<Guid>
                }

              
                await _userManager.AddToRoleAsync(user, defaultRole);

                
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, defaultRole));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                await _userManager.AddClaimAsync(user, new Claim("IsAdmin", "false"));
                await _userManager.AddClaimAsync(user, new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
                await _userManager.AddClaimAsync(user, new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
            }
            else
            {
                
            }

            return (result,user);
        }
    }
}
