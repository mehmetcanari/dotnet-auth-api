using Auth.Infrastructure.Context;
using Auth.WebAPI;
using Microsoft.EntityFrameworkCore;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddDbContext<ApplicationDatabaseContext>(options =>
        {
            options.UseInMemoryDatabase("AuthDB");
        });

        DIContainer container = new DIContainer(builder.Services);
        container.RegisterServices();

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}