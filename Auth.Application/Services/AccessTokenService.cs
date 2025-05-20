using Auth.Application.Abstract;
using Auth.Application.Utility;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly IAccountRepository _accountRepository;
    private const double AccessTokenExpirationTime = 30;
    private readonly ICurrentUserService _currentUserService;

    public AccessTokenService(
        IAccountRepository accountRepository, 
        ICurrentUserService currentUserService)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<AccessToken> GenerateAccessTokenAsync()
    {
        try
        {
            var result = _currentUserService.GetCurrentUserEmail() ?? throw new Exception("User email not found.");
            if (result.Data != null)
            {
                Account account = await _accountRepository.GetAccountByEmailAsync(result.Data);

                if (account == null)
                {
                    throw new KeyNotFoundException($"Account with email {result.Data} not found.");
                }
            
                AccessToken token = new AccessToken
                {
                    Token = TokenGenerateProvider.GenerateToken(account.Email, account.Role, AccessTokenExpirationTime, TokenGenerateProvider.TokenType.Access),
                    Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationTime)
                };
            
                return token;
            }
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while generating the refresh token.", e);
        }
        
        throw new Exception("An error occurred while generating the access token.");
    }
}