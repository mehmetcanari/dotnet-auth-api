using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IRefreshTokenService
{
    Task GenerateRefreshTokenAsync(string email);
    Task RevokeRefreshTokenAsync(RefreshToken token);
}