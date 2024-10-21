using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.Extensions.DependencyInjection;
using HUBTSOCIAL.src.Features.Auth.Models;
using MongoDbGenericRepository;
using HUBTSOCIAL.src.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBTSOCIAL.src.Configurations
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, MongoIdentityRole<Guid>>()
                .AddMongoDbStores<ApplicationUser, MongoIdentityRole<Guid>, Guid>(
                    configuration.GetConnectionString("AuthService"), "HUBT_Management")
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
