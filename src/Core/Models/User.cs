using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
namespace HUBTSOCIAL.src.Core.Models
{
    [CollectionName("User")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string FullName { get; set; } = String.Empty;

        public string? VerificationCode { get; set; } = String.Empty;
        public DateTime VerificationCodeExpires { get; set; }
        public bool IsLoggedIn  { get; set; } = false;
    }
}