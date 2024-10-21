
using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;
using Microsoft.AspNetCore.Identity;

namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface ILoginService
    {
        Task<(SignInResult,ApplicationUser?)> LoginAsync(LoginRequest model);
    }
}