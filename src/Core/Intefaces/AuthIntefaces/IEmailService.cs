namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface IEmailService
    {
        Task<(bool success, string? code)> SendVerificationCodeEmail(string email);
    
    }   
}
