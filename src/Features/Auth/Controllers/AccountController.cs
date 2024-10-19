using Microsoft.AspNetCore.Mvc;
using HUBTSOCIAL.src.Features.Auth.Responses;
using HUBTSOCIAL.src.Features.Auth.Requests;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;

namespace HUBTSOCIAL.src.Features.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
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

            var (result,user) = await _accountService.RegisterAsync(request);
            if (!result.Succeeded)
            {
                return BadRequest(
                    new AụthResponse(
                        success:false,
                        statusCode: 400,
                        message: "Đăng ký thất bại.", result.Errors));
            }

            // Gửi mã OTP qua email để xác thực
            var succeeded = await _accountService.SendOtpAndUpdateUserAsync(user, request.Email);
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
                    message: "Đăng ký thành công. Vui lòng kiểm tra email để nhập mã OTP."
                )
            );

        }

        // Đăng nhập và gửi mã OTP qua email
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest model)
        {
            var (result,user) = await _accountService.LoginAsync(model);

            if (result.Succeeded && user is not null)
            {
                #pragma warning disable CS8604 // Possible null reference argument.
                var succeeded= await _accountService.SendOtpAndUpdateUserAsync(user, user.Email);
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

            var result = await _accountService.VerifyCodeAsync(request);
            if (result)
            {
                // Tạo token cho người dùng
                var user = await _accountService.FindUserByUserNameAsync(request.UserName);
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


    }

    
}
