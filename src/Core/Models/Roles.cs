using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
namespace HUBTSOCIAL.src.Core.Models
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
        
    }
}
