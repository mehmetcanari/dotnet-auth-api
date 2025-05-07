using Auth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Application.Abstract;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(string email);
    Task<string> ValidateRefreshTokenAsync(string token);
    Task RemoveRefreshTokenAsync(string email);
}