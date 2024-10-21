using System.Security.Claims;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Features.Auth.Models;
using HUBTSOCIAL.src.Core.Models;
namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user,string? refreshToken);

    }    
}

