using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FoodBook_SS.Domain.Entities.User;
using FoodBook_SS.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace FoodBook_SS.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly int _expiresMinutes;
        private readonly string _audience;

        public JwtTokenService(IConfiguration config)
        {
            _secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret no configurado.");
            _issuer = config["Jwt:Issuer"] ?? "FoodBookPro";
            _expiresMinutes = int.Parse(config["Jwt:ExpiresMinutes"] ?? "60");
            _audience = config["Jwt:Audience"] ?? "FoodBookProUsers";
        }

        public (string Token, string RefreshToken, DateTime ExpiresAt) Generate(Usuario usuario)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_expiresMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email,            usuario.Email),
                new Claim(ClaimTypes.Role,             usuario.Rol?.Nombre ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return (
                new JwtSecurityTokenHandler().WriteToken(token),
                GenerateRefreshToken(),
                expiresAt
            );
        }

        public int? ValidateAndGetUserId(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validated);

                var jwt = (JwtSecurityToken)validated;
                return int.Parse(jwt.Subject);
            }
            catch { return null; }
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
