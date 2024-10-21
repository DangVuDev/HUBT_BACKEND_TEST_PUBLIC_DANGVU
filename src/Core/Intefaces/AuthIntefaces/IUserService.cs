using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Requests;

namespace HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices
{
    public interface IUserService
    {
        Task<ApplicationUser> FindUserByUserNameAsync(string userName);
        Task<bool> PromoteUserAccountAsync(string userName, string roleName);
        Task<bool> ChangeLanguage(ChangeLanguageRequest changeLanguageRequest);
    }
}