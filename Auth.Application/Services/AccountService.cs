using Auth.Application.Abstract;
using Auth.Application.DTO;
using Auth.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger, ICurrentUserService currentUserService)
    {
        _accountRepository = accountRepository;
        _logger = logger;
        _currentUserService = currentUserService;
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
    
    public async Task<AccountResponseDto> GetAccountByEmailAsync()
    {
        try
        {
            var result = _currentUserService.GetCurrentUserEmail();
            if (result.Data == null)
            {
                throw new Exception("User email not found.");
            }
            
            var account = await _accountRepository.GetAccountByEmailAsync(result.Data);

            AccountResponseDto accountResponse = new AccountResponseDto
            {
                Name = account.Name,
                Surname = account.Surname,
                Email = account.Email,
                Role = account.Role,
            };

            _logger.LogInformation("Account retrieved: {Email}", result.Data);
            return accountResponse;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving account: {Email}", _currentUserService.GetCurrentUserEmail());
            throw;
        }
    }
}