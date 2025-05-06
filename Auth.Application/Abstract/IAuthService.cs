using Auth.Application.Common.Responses;
using Auth.Application.DTO;

namespace Auth.Application.Abstract;

public interface IAuthService
{
    Task<bool> RegisterAsync(AccountRegisterRequestDto accountRegisterRequestDto);
    Task<AuthLoginResponseDto> LoginAsync(AccountLoginRequestDto accountLoginRequestDto);
}