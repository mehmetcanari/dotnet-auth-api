using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenRepository
{
    Task<List<RefreshToken>> GetRefreshTokensAsync(string email);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    Task<List<RefreshToken>> GetActiveRefreshTokensAsync();
}