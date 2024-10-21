namespace HUBTSOCIAL.src.Features.Auth.Models
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
        public string Audience { get; set; } = String.Empty;
        public int TokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInDays { get; set; }
    }

}