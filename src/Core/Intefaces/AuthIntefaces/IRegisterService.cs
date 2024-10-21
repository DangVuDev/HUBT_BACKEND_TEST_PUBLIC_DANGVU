using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;
using Microsoft.AspNetCore.Identity;

namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface IRegisterService
    {
        Task<(IdentityResult,ApplicationUser)> RegisterAsync(RegisterRequest model);
    }
}