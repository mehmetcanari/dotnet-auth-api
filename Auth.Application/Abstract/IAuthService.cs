using System.Security.Claims;
using Auth.Application.DTO;

namespace Auth.Application.Abstract;

public interface IAuthService
{
    Task RegisterAsync(AccountRegisterRequestDto accountRegisterRequestDto);
    Task<AuthLoginResponseDto> LoginAsync(AccountLoginRequestDto accountLoginRequestDto);
    Task LogoutAsync();
}