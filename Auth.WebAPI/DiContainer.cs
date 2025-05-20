using Auth.Application.Abstract;
using Auth.Application.Services;
using Auth.Infrastructure.Repositories;
namespace Auth.WebAPI;
public class DiContainer(IServiceCollection services)
{
    public void RegisterServices()
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }
}