using System.ComponentModel.DataAnnotations;

namespace HUBTSOCIAL.src.Features.Auth.Requests
{
    public class ConfirmRequest
    {
        [Required]
        public string UserName { get; set; } = String.Empty;
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có độ dài 6 ký tự.")]
        public string OTP { get; set; } = String.Empty;
    }
}