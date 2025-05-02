using Auth.Application.Abstract;
using Auth.Infrastructure.Repositories;
namespace Auth.WebAPI;
public class DIContainer
{
    private readonly IServiceCollection _services;

    public DIContainer(IServiceCollection services)
    {
        _services = services;
    }

    public void RegisterServices()
    {
        _services.AddScoped<IAccountRepository, AccountRepository>();
        _services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }
}