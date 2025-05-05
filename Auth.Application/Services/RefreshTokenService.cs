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

    public async Task GenerateRefreshTokenAsync(int accountId)
    {
        try
        {
            Account account = await _accountRepository.GetAccountByIdAsync(accountId);
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
                AccountId = accountId,
            };
            
            await _refreshTokenRepository.AddRefreshTokenAsync(token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while generating the refresh token for account ID {AccountId}", accountId);
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
            _logger.LogError(e, "An error occurred while validating the refresh token for account ID {AccountId}", accountId);
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
            _logger.LogError(e, "An error occurred while revoking the refresh token for account ID {AccountId}", token.AccountId);
            throw new Exception("An error occurred while revoking the refresh token.", e);
        }
    }
}