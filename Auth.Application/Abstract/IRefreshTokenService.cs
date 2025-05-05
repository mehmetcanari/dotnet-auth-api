using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenService
{
    Task GenerateRefreshTokenAsync(int accountId);
    Task<bool> ValidateRefreshTokenAsync(int accountId);
    Task RevokeRefreshTokenAsync(RefreshToken token);
}