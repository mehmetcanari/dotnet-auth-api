using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Utility;

public static class TokenGenerateProvider
{
    private static DateTime _expiration;

    public static string GenerateToken(string email, string role, double expiresIn, TokenType type)
    {
        try
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey ??
                                       throw new InvalidOperationException("JWT_SECRET is not configured")));

            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
                          ?? throw new InvalidOperationException("JWT_ISSUER is not configured");
            
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
                            ?? throw new InvalidOperationException("JWT_AUDIENCE is not configured");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("tokenType", type == TokenType.Access ? "access" : "refresh"),
                new(ClaimTypes.Role, role)
            };

            switch (type)
            {
                case TokenType.Access:
                    _expiration = DateTime.UtcNow.AddMinutes(expiresIn);
                    break;
                case TokenType.Refresh:
                    _expiration = DateTime.UtcNow.AddDays(expiresIn);
                    break;
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: _expiration,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while generating the token.", ex);
        }
    }

    public enum TokenType
    {
        Access,
        Refresh
    }
}