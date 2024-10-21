using System.ComponentModel.DataAnnotations;

namespace HUBTSOCIAL.src.Features.Auth.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        [StringLength(10, ErrorMessage = "")]
        public string UserName { get; set; } = String.Empty; // UserName sẽ sử dụng mã sinh viên

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = String.Empty; // Email dùng để nhận mã xác nhận khi xác thực hai bước

        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự.")]
        public string FullName { get; set; } = String.Empty; // Hiển thị tên đầy đủ của tài khoản

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = String.Empty; // Mật khẩu
    }    
}


