using Auth.Application.Abstract;
using Auth.Application.DTO;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }
    
    public async Task<bool> RegisterAccountAsync(AccountRegisterRequestDto accountRegisterRequestDto)
    {
        try
        {
            var account = new Account
            {
                Name = accountRegisterRequestDto.Name,
                Surname = accountRegisterRequestDto.Surname,
                Email = accountRegisterRequestDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Role = "User",
            };

            await _accountRepository.AddAccountAsync(account);
            _logger.LogInformation("Account registered: {Email}", account.Email);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error registering account: {Email}", accountRegisterRequestDto.Email);
            throw;
        }
    }
    
    public async Task<AccountResponseDto> GetAccountByEmailAsync(string email)
    {
        try
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);

            AccountResponseDto accountResponse = new AccountResponseDto
            {
                Name = account.Name,
                Surname = account.Surname,
                Email = account.Email,
                Role = account.Role,
            };

            _logger.LogInformation("Account retrieved: {Email}", email);
            return accountResponse;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving account: {Email}", email);
            throw;
        }
    }
    
    public async Task RemoveAccountAsync(string email)
    {
        try
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);
            await _accountRepository.RemoveAccountAsync(account);
            _logger.LogInformation("Account removed: {Email}", email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing account: {Email}", email);
            throw;
        }
    }
}