using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync();
    Task RemoveRefreshTokenAsync(string email);
}