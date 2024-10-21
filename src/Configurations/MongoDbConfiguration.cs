using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using HUBTSOCIAL.src.Features.Auth.Models;
using HUBTSOCIAL.src.Features.Chat.Models;

namespace HUBTSOCIAL.src.Configurations
{
    public static class MongoDbConfiguration
    {
        public static IServiceCollection ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMongoCollection<RefreshToken>>(s =>
            {
                var client = new MongoClient(configuration.GetConnectionString("AuthService"));
                var database = client.GetDatabase("HUBT_Management");
                return database.GetCollection<RefreshToken>("RefreshTokens");
            });

            services.AddScoped<IMongoCollection<ChatRoomModel>>(s =>
            {
                var chatClient = new MongoClient(configuration.GetConnectionString("ChatService"));
                var chatDatabase = chatClient.GetDatabase("HUBT_Management");
                return chatDatabase.GetCollection<ChatRoomModel>("ChatRoom");
            });

            return services;
        }
    }
}
