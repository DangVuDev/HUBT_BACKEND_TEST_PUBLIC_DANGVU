using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HUBTSOCIAL.src.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using HUBTSOCIAL.src.Features.Auth.Services.IntefaceServices;
using HUBTSOCIAL.src.Core.Models;

namespace HUBTSOCIAL.src.Features.Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;

        public TokenService(
            UserManager<ApplicationUser> userManager,       
            IOptions<JwtSettings> jwtSettings, 
            IMongoCollection<RefreshToken> refreshTokenCollection
        )
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _refreshTokenCollection = refreshTokenCollection;
        }

        // Tạo JWT token và handle Refresh Token
        public async Task<string> GenerateTokenAsync(ApplicationUser user, string? refreshToken)
        {
            List<Claim> claims = new();

            claims.AddRange(await _userManager.GetClaimsAsync(user));

            IList<string>? roles = await _userManager.GetRolesAsync(user);
            IEnumerable<Claim>? roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(roleClaims);

            // Tạo JWT token
            string? token = GenerateJwtToken(claims);

            // Xử lý Refresh Token: Cập nhật hoặc tạo mới
            await HandleRefreshTokenAsync(user, token);

            return token;
        }
        
        private async Task HandleRefreshTokenAsync(ApplicationUser user, string accessToken)
        {
            RefreshToken? existingRefreshToken = await _refreshTokenCollection
                .Find(t => t.UserId == user.Id.ToString())
                .FirstOrDefaultAsync();

            if (existingRefreshToken != null)
            {
                // Cập nhật Refresh Token
                UpdateDefinition<RefreshToken>? update = Builders<RefreshToken>.Update
                    .Set(t => t.AccessToken, accessToken)
                    .Set(t => t.Expires, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays));
                await _refreshTokenCollection.UpdateOneAsync(t => t.UserId == user.Id.ToString(), update);
            }
            else
            {
                // Tạo mới Refresh Token
                RefreshToken? newRefreshToken = new RefreshToken
                {
                    UserId = user.Id.ToString(),
                    AccessToken = accessToken,
                    Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
                };
                await _refreshTokenCollection.InsertOneAsync(newRefreshToken);
            }
        }

        // Tạo JWT Token
        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            SymmetricSecurityKey? securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            SigningCredentials? credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor? tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
