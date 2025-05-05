using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IAccessTokenService
{
    Task<AccessToken> GenerateAccessTokenAsync(int accountId);
}