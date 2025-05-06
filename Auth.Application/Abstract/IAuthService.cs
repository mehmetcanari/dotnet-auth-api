using Auth.Application.DTO;

namespace Auth.Application.Abstract;

public interface IAuthService
{
    Task<string> RegisterAsync(AccountRegisterRequestDto accountRegisterRequestDto);
    Task<AuthLoginResponseDto> LoginAsync(AccountLoginRequestDto accountLoginRequestDto);
}