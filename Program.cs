using HUBTSOCIAL.src.Configurations;
using HUBTSOCIAL.src.Features.Chat.ChatHubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace HUBTSOCIAL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Cấu hình
            builder.Services.ConfigureLocalization();
            builder.Services.ConfigureJwt(builder.Configuration);
            builder.Services.ConfigureIdentity(builder.Configuration);
            builder.Services.ConfigureMongoDb(builder.Configuration);
            builder.Services.ConfigureSignalR();
            builder.Services.ConfigureCloudinary(builder.Configuration);

            // Đăng ký các dịch vụ của hệ thống
            builder.Services.RegisterApplicationServices();

            // Thêm hỗ trợ cho các controller
            builder.Services.AddControllers();

            var app = builder.Build();

            // Cấu hình HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Định tuyến các controller và SignalR Hub
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
