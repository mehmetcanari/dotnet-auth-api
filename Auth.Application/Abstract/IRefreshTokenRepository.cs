using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetAccountToken(int accountId);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
}