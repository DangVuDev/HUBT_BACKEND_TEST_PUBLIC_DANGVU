using System.ComponentModel.DataAnnotations;
namespace HUBTSOCIAL.src.Features.Auth.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Tên người dùng là mã sinh viên")]
        [StringLength(10, ErrorMessage = "")]
        public string UserName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = String.Empty;
    }   
}


