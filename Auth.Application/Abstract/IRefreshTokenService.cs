using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(string email);
    Task<string> ValidateRefreshTokenAsync(string token);
    Task RemoveRefreshTokenAsync(string email);
}