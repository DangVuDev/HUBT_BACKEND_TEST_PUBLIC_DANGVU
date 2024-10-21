using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace HUBTSOCIAL.src.Configurations
{
    public static class LocalizationConfig
    {
        public static void ConfigureLocalization(this IServiceCollection services)
        {
            // Đăng ký các ngôn ngữ hỗ trợ và cấu hình localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("vi"), new CultureInfo("en") };

                options.DefaultRequestCulture = new RequestCulture("vi"); // Ngôn ngữ mặc định
                options.SupportedCultures = supportedCultures;           // Các ngôn ngữ hỗ trợ
                options.SupportedUICultures = supportedCultures;         // Các UI cultures hỗ trợ
            });
        }

        public static void UseLocalization(this IApplicationBuilder app)
        {
            // Áp dụng middleware hỗ trợ đa ngôn ngữ
            var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(options);
        }
    }
}
