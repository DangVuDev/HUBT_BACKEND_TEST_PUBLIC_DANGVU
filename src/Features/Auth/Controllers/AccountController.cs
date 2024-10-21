using Microsoft.AspNetCore.Mvc;
using HUBTSOCIAL.src.Features.Auth.Responses;
using HUBTSOCIAL.src.Features.Auth.Requests;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;
using HUBTSOCIAL.src.Core.Languages;
using HUBTSOCIAL.src.Features.Auth.Services;
using HUBTSOCIAL.src.Core.Models;

namespace HUBTSOCIAL.src.Features.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly VerificationService _verificationService;
        private readonly TokenService _tokenService;

        private readonly LocalizationService _localization;

        public AccountController(IAuthService authService, LocalizationService localizationService, VerificationService verificationService, TokenService tokenService)
        {
            _authService = authService;
            _localization = localizationService;
            _verificationService = verificationService;
            _tokenService = tokenService;
        }

        // Đăng ký tài khoản mới và gửi mã OTP qua email
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 400,
                        message: "Thông tin không hợp lệ."
                    )
                );
            }

            var (result,user) = await _authService.RegisterAsync(request);
            if (!result.Succeeded)
            {
                return BadRequest(
                    new AụthResponse(
                        success:false,
                        statusCode: 400,
                        message: "Đăng ký thất bại.",
                        result.Errors));
            }

            // Gửi mã OTP qua email để xác thực
            bool succeeded = await _verificationService.SendOtpAndUpdateUserAsync(user);
            if (!succeeded)
            {
                return StatusCode(
                    500,
                    new AụthResponse(
                        success: false,
                        statusCode: 500,
                        message: _localization.GetMessage("UnableSendOTP")
                    )
                );
            }
            
            return Ok(
                new AụthResponse(
                    success: true,
                    statusCode: 200,
                    message: "Đăng ký thành công. Vui lòng kiểm tra email để nhập mã OTP."
                )
            );

        }

        // Đăng nhập và gửi mã OTP qua email
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest model)
        {
            var (result,user) = await _authService.LoginAsync(model);

            if (result.Succeeded && user is not null)
            {
                #pragma warning disable CS8604 // Possible null reference argument.
                bool succeeded= await _verificationService.SendOtpAndUpdateUserAsync(user);
                #pragma warning restore CS8604 // Possible null reference argument.

                if (!succeeded)
                {
                    return StatusCode(
                        500,
                        new AụthResponse(
                            success: false,
                            statusCode: 500,
                            message: "Không thể gửi mã OTP. Vui lòng thử lại."
                        )
                    );
                }

                
                return Ok(
                    new AụthResponse(
                        success: true,
                        statusCode: 200,
                        message: "Xác thực bước một thành công. Vui lòng kiểm tra email để nhập mã OTP."
                    )
                );

            }
            else if (result.IsLockedOut)
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 403,
                        message: "Tài khoản bị khóa."
                    )
                );
            }
            else if (result.IsNotAllowed)
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 403,
                        message: "Đăng nhập không được phép."
                    )
                );
            }
            else if (result.RequiresTwoFactor)
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 401,
                        message: "Yêu cầu xác thực hai yếu tố."
                    )
                );
            }
            else
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 400,
                        message: "Tên đăng nhập hoặc mật khẩu không đúng."
                    )
                );
            }

        }

        // Xác thực mã OTP và tạo token nếu thành công
        [HttpPost("confirm-code")]
        public async Task<IActionResult> ConfirmCode([FromBody] ConfirmRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 400,
                        message: "Thông tin không hợp lệ."
                    )
                );
            }

            ApplicationUser? user = await _authService.VerifyCodeAsync(request);
            if (user is not null)
            {
                var token = await _tokenService.GenerateTokenAsync(user, null);
                
                return Ok(
                    new AụthResponse(
                        success: true,
                        statusCode: 200,
                        message: "Xác thực thành công.",
                        data: new { Token = token }
                    )
                );
            }

            return Unauthorized(
                new AụthResponse(
                    success: false,
                    statusCode: 401,
                    message: "Xác thực mã OTP không thành công."
                )
            );
        }

        // Thay đổi ngôn ngữ
        [HttpPost("change-language")]
        public async Task<IActionResult> ChangeLanguage([FromBody] ChangeLanguageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Language))
            {
                return BadRequest(
                    new AụthResponse(
                        success: false,
                        statusCode: 400,
                        message: _localization.GetMessage("InvalidLanguage")
                    )
                );
            }

            // Gọi phương thức trong AccountService để thay đổi ngôn ngữ
            bool result = await _authService.ChangeLanguage(request);
            if (result)
            {
                return Ok(
                    new AụthResponse(
                        success: true,
                        statusCode: 200,
                        message: _localization.GetMessage("LanguageChanged")
                    )
                );
            }

            return BadRequest(
                new AụthResponse(
                    success: false,
                    statusCode: 400,
                    message: _localization.GetMessage("LanguageChangeFailed")
                )
            );
        }
    }

    
}
