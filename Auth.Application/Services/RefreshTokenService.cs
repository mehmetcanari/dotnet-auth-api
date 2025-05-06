using Auth.Application.Abstract;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;
using Auth.Application.Utility;

namespace Auth.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccountRepository _accountRepository;
    private const double RefreshTokenExpirationTime = 30;
    
    public RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository, IAccountRepository accountRepository)
    {
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _accountRepository = accountRepository;
    }

    public async Task GenerateRefreshTokenAsync(string email)
    {
        try
        {
            Account account = await _accountRepository.GetAccountByEmailAsync(email);
            
            if (account == null)
            {
                throw new KeyNotFoundException($"Account with email {email} not found.");
            }
            
            List<RefreshToken> activeTokens = await _refreshTokenRepository.GetActiveRefreshTokensAsync();

            if (activeTokens.Count != 0)
            {
                foreach (RefreshToken refreshToken in activeTokens)
                {
                    refreshToken.Revoke();
                }
            }
            
            RefreshToken token = new RefreshToken
            {
                Token = TokenGenerateProvider.GenerateToken(account.Email, account.Role, RefreshTokenExpirationTime, TokenGenerateProvider.TokenType.Refresh),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationTime),
                Email = account.Email,
            };
            
            await _refreshTokenRepository.AddRefreshTokenAsync(token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while generating the refresh token for account email {email}", email);
            throw new Exception("An error occurred while generating the refresh token.", e);
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
            _logger.LogError(e, "An error occurred while revoking the refresh token for account email {email}", token.Email);
            throw new Exception("An error occurred while revoking the refresh token.", e);
        }
    }
}