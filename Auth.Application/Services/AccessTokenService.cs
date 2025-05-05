using Auth.Application.Abstract;
using Auth.Application.Utility;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly IAccountRepository _accountRepository;
    private const double AccessTokenExpirationTime = 30;

    public AccessTokenService(ILogger<RefreshTokenService> logger, IAccountRepository accountRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
    }

    public async Task<AccessToken> GenerateAccessTokenAsync(int accountId)
    {
        try
        {
            Account account = await _accountRepository.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                _logger.LogWarning($"Account with ID {accountId} not found.");
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            }
            
            AccessToken token = new AccessToken
            {
                Token = TokenGenerateProvider.GenerateToken(account.Email, account.Role, AccessTokenExpirationTime, TokenGenerateProvider.TokenType.Access),
                Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationTime)
            };
            
            return token;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while generating the access token for account ID {AccountId}", accountId);
            throw new Exception("An error occurred while generating the refresh token.", e);
        }
    }
}