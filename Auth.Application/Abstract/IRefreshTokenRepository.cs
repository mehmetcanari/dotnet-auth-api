using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetRefreshTokenAsync(int accountId);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
}