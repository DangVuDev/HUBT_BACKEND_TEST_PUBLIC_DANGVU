using Microsoft.Extensions.DependencyInjection;
using HUBTSOCIAL.src.Features.Auth.Services;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;
using HUBTSOCIAL.src.Features.Chat.Services;
using HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces;
using HUBTSOCIAL.src.Core.Interfaces.IHubs;
using HUBTSOCIAL.src.Core.Languages;
using HUBTSOCIAL.src.Features.Chat.Services.ChildChatServices;
using HUBTSOCIAL.src.Features.Chat.Hubs.ChildChatHubs;

namespace HUBTSOCIAL.src.Configurations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các dịch vụ
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IChatMessageHub, ChatMessageHub>();
            services.AddSingleton<LocalizationService>();
            services.AddScoped<IChatFileHub, ChatFileHub>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVerificationService, VerificationService>();

            

            return services;
        }
    }
}
