using System.Security.Claims;
using System.Text;
using Auth.Application.Abstract;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccountRepository _accountRepository;
    
    public RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository, IAccountRepository accountRepository)
    {
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _accountRepository = accountRepository;
    }

    public async Task GenerateRefreshTokenAsync(int accountId)
    {
        try
        {
            Account account = await _accountRepository.GetAccountByIdAsync(accountId);
            
            RefreshToken token = new RefreshToken
            {
                Token = GenerateToken(account.Email, account.Role),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                AccountId = accountId,
            };
            
            await _refreshTokenRepository.AddRefreshTokenAsync(token);
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while generating the refresh token.", e);
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(int accountId)
    {
        try
        {
            Account account = await _accountRepository.GetAccountByIdAsync(accountId);
            RefreshToken token = await _refreshTokenRepository.GetRefreshTokenAsync(accountId);

            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            }

            if (token.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning($"Refresh token for account ID {accountId} has expired.");
                return false;
            }

            if (token.IsRevoked)
            {
                _logger.LogWarning($"Refresh token for account ID {accountId} has been revoked.");
                return false;
            }
            
            return true;
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while validating the refresh token.", e);
        }
    }

    public Task RevokeRefreshTokenAsync(RefreshToken token)
    {
        try
        {
            token.Revoke();
            return _refreshTokenRepository.UpdateRefreshTokenAsync(token);
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while revoking the refresh token.", e);
        }
    }

    private string GenerateToken(string email, string role)
    {
        try
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("JWT_SECRET is not configured")));

            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var expirationMinutes = Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("tokenType", "access"),
                new(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expirationMinutes)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating access token");
            throw;
        }
    }
}