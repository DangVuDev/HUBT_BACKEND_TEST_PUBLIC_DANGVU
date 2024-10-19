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

    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<MongoIdentityRole<Guid>> _roleManager; 
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<ApplicationUser> userManager,
                            RoleManager<MongoIdentityRole<Guid>> roleManager,
                            SignInManager<ApplicationUser> signInManager,
                            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<(IdentityResult,ApplicationUser)> RegisterAsync(RegisterRequest model)
        {
            // Kiểm tra null cho model
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model không thể null.");
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

       
            var result = await _userManager.CreateAsync(user, model.Password);

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

        public async Task<(SignInResult,ApplicationUser?)> LoginAsync(LoginRequest model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);           
            return result.Succeeded 
                ? (result,await _userManager.FindByNameAsync(model.UserName)) 
                : (result,null);
        }





        public async Task<bool> PromoteUserAccountAsync(string userName, string roleName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName), "Tên người dùng không thể null hoặc rỗng.");
            }

            if (string.IsNullOrEmpty(roleName) || 
                (roleName != "USER" && roleName != "TEACHER" && roleName != "HEAD" && roleName != "ADMIN"))
            {
                throw new ArgumentException("Giá trị vai trò không hợp lệ. Nó phải là USER, TEACHER, HEAD hoặc ADMIN.", nameof(roleName));
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new MongoIdentityRole<Guid>(roleName);
                    await _roleManager.CreateAsync(role);
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);
                return result.Succeeded;
            }

            return false;
        }


        public async Task<bool> PromoteToTeacherAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName), "Tên người dùng không thể null hoặc rỗng.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                const string adminRole = "TEACHER";

                if (!await _roleManager.RoleExistsAsync(adminRole))
                {
                    var role = new MongoIdentityRole<Guid>(adminRole);
                    await _roleManager.CreateAsync(role);
                }

                // Thêm người dùng vào vai trò ADMIN
                var result = await _userManager.AddToRoleAsync(user, adminRole);
                return result.Succeeded;
            }

            // Trả về false nếu không tìm thấy người dùng
            return false;
        }


        public async Task<bool> VerifyCodeAsync(ConfirmRequest request)
        {

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                Console.WriteLine($"User not found: {request.UserName}");
                return false; 
            }

            if (user.VerificationCode != request.OTP || user.VerificationCodeExpires < DateTime.UtcNow)
            {

                Console.WriteLine($"Verification failed for user: {request.UserName}. Code: {request.OTP}");
                return false; 
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
                return false; 
            }

            Console.WriteLine($"User {request.UserName} verified successfully.");
            return true; 
        }

        public async Task<ApplicationUser> FindUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Tên người dùng không được để trống.", nameof(userName));

            #pragma warning disable CS8603 
            return await _userManager.FindByNameAsync(userName);
            #pragma warning restore CS8603 
        }
        public async Task<bool> SendOtpAndUpdateUserAsync(ApplicationUser user, string email)
        {
            // Gửi mã OTP qua email
            var (success, code) = await _emailService.SendVerificationCodeEmail(email);
            if (!success || string.IsNullOrEmpty(code))
            {
                return false;
            }

            // Cập nhật mã OTP cho người dùng
            var updated = await UpdateVerificationUserAsync(user, code);

            return updated ? true : false;
        }

        private async Task<bool> UpdateVerificationUserAsync(ApplicationUser user, string code)
        {
            user.VerificationCode = code;
            user.VerificationCodeExpires = DateTime.UtcNow.AddMinutes(5);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return false;
            }         
            return true;
        }

    }
}