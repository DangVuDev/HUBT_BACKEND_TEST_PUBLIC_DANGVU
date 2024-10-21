using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
namespace HUBTSOCIAL.src.Core.Models
{
    [CollectionName("User")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string FullName { get; set; } = String.Empty;
        public bool Gender { get; set; } // ture  is male, false is female
        public string Hometown { get; set; } = String.Empty;
        public string NumberPhone { get; set; } = String.Empty;
        public DateTime DateOfBorn { get; set; }
        public string Language { get; set; } = "vn";
        public string? VerificationCode { get; set; } = String.Empty;
        public DateTime VerificationCodeExpires { get; set; }

    }
}