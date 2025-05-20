using System.Security.Claims;
using Auth.Application.Abstract;
using Auth.Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Auth.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Result<string> GetCurrentUserEmail()
    {
        string? email = TryGetEmailFromClaims();

        if (string.IsNullOrEmpty(email))
        {
            return Result<string>.Failure("Unauthorized", "User is not authenticated.");
        }

        return Result<string>.Success(email);
    }

    private string? TryGetEmailFromClaims()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return null;

        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            email = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        return email;
    }

    public Result<bool> IsAuthenticated()
    {
        var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            return Result<bool>.Failure("Unauthorized", "User is not authenticated.");
        }

        return Result<bool>.Success(isAuthenticated);
    }
}