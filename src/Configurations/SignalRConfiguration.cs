using Microsoft.Extensions.DependencyInjection;

namespace HUBTSOCIAL.src.Configurations
{
    public static class SignalRConfiguration
    {
        public static IServiceCollection ConfigureSignalR(this IServiceCollection services)
        {
            // Đăng ký SignalR
            services.AddSignalR();
            return services;
        }
    }
}
