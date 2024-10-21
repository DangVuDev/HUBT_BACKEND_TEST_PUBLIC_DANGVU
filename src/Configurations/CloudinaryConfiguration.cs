using Microsoft.Extensions.DependencyInjection;
using CloudinaryDotNet;

namespace HUBTSOCIAL.src.Configurations
{
    public static class CloudinaryConfiguration
    {
        public static IServiceCollection ConfigureCloudinary(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Cloudinary>(s =>
            {
                var config = configuration.GetSection("Cloudinary");
                var account = new Account
                (
                    config["CloudName"],
                    config["ApiKey"],
                    config["ApiSecret"]
                );
                return new Cloudinary(account);
            });

            return services;
        }
    }
}
