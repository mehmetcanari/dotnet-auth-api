using Auth.Application.DTO;

namespace Auth.Application.Abstract;

public interface IAccountService
{
    Task<bool> RegisterAccountAsync(AccountRegisterRequestDto accountRegisterRequestDto);
    Task<AccountResponseDto> GetAccountByEmailAsync();
}