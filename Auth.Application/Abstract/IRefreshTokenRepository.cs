using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetRefreshTokenByTokenAsync(string token);
    Task<List<RefreshToken>> GetActiveRefreshTokensAsync(string email);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task RemoveRefreshTokensAsync(string email);
}