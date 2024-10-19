using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDbGenericRepository;
using HUBTSOCIAL.src.Features.Auth.Models;
using HUBTSOCIAL.src.Core.Models;
using HUBTSOCIAL.src.Features.Auth.Services;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using HUBTSOCIAL.src.Features.Chat.Models;
using CloudinaryDotNet;
using HUBTSOCIAL.src.Core.Interfaces.ChatInterfaces;
using HUBTSOCIAL.src.Features.Chat.Services;
using HUBTSOCIAL.src.Features.Chat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình JWT (Token)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
#pragma warning disable CS8602
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
#pragma warning restore CS8602
});

// Đăng ký Identity với MongoDB
builder.Services.AddIdentity<ApplicationUser, MongoIdentityRole<Guid>>()
    .AddMongoDbStores<ApplicationUser, MongoIdentityRole<Guid>, Guid>(
        builder.Configuration.GetConnectionString("AuthService"), "HUBT_Management")
    .AddDefaultTokenProviders();

// Cấu hình MongoDB cho RefreshToken và ChatRoom
builder.Services.AddScoped<IMongoCollection<RefreshToken>>(s =>
{
    var client = new MongoClient(builder.Configuration.GetConnectionString("AuthService"));
    var database = client.GetDatabase("HUBT_Management");
    return database.GetCollection<RefreshToken>("RefreshTokens");
});

builder.Services.AddScoped<IMongoCollection<ChatRoom>>(s =>
{
    var chatClient = new MongoClient(builder.Configuration.GetConnectionString("ChatService"));
    var chatDatabase = chatClient.GetDatabase("HUBT_Management");
    return chatDatabase.GetCollection<ChatRoom>("ChatRoom");
});

// Đăng ký SignalR và Cloudinary
builder.Services.AddSignalR();

builder.Services.AddScoped<Cloudinary>(s =>
{
    var config = builder.Configuration.GetSection("Cloudinary");
    var account = new Account
    (
        config["CloudName"],
        config["ApiKey"],
        config["ApiSecret"]
    );
    return new Cloudinary(account);
});

// Đăng ký các dịch vụ cho hệ thống
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IChatMessageHub, ChatMessageHub>();
builder.Services.AddScoped<IChatFileHub, ChatFileHub>();

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
