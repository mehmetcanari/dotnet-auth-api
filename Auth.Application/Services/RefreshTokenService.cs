using Auth.Application.Abstract;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;
using Auth.Application.Utility;
using Microsoft.AspNetCore.Http;

namespace Auth.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccountRepository _accountRepository;
    private const double RefreshTokenExpirationTime = 30;

    public RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository,
        IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _accountRepository = accountRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(string email)
    {
        try
        {
            Account account = await _accountRepository.GetAccountByEmailAsync(email);

            if (account == null)
            {
                throw new KeyNotFoundException($"Account with email {email} not found.");
            }

            await RevokeActiveTokensAsync(email);

            RefreshToken token = new RefreshToken
            {
                Token = TokenGenerateProvider.GenerateToken(account.Email, account.Role, RefreshTokenExpirationTime,
                    TokenGenerateProvider.TokenType.Refresh),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationTime),
                Email = account.Email,
            };

            AddRefreshTokenToCookie(token.Token, token.ExpiresAt);
            await _refreshTokenRepository.AddRefreshTokenAsync(token);

            return token;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while generating the refresh token for account email {email}",
                email);
            throw new Exception("An error occurred while generating the refresh token.", e);
        }
    }

    private async Task RevokeActiveTokensAsync(string email)
    {
        try
        {
            List<RefreshToken> activeTokens = await _refreshTokenRepository.GetActiveRefreshTokensAsync(email);

            if (activeTokens.Count != 0)
            {
                foreach (RefreshToken refreshToken in activeTokens)
                {
                    refreshToken.Revoke();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while revoking active refresh tokens.");
            throw new Exception("An error occurred while revoking active refresh tokens.", e);
        }
    }

    public async Task RemoveRefreshTokenAsync(string email)
    {
        try
        {
            RemoveRefreshTokenFromCookie();
            await _refreshTokenRepository.RemoveRefreshTokensAsync(email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while removing the refresh token for account email {email}", email);
            throw new Exception("An error occurred while removing the refresh token.", e);
        }
    }

    public async Task<string> ValidateRefreshTokenAsync(string token)
    {
        try
        {
            RefreshToken refreshToken = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(token);
            return refreshToken.Email;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while validating the refresh token: {token}", token);
            throw new Exception("An error occurred while validating the refresh token.", e);
        }
    }

    private void AddRefreshTokenToCookie(string token, DateTime expire)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext?.Response.Cookies.Append("refreshToken", token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expire,
            });
    }

    private void RemoveRefreshTokenFromCookie()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext?.Response.Cookies.Delete("refreshToken");
    }
}