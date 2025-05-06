using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Persistence.Context;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth.WebAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load();        
        
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_SECRET");
        builder.Configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
        builder.Configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        builder.Services.AddDbContext<ApplicationDatabaseContext>(options =>
        {
            options.UseInMemoryDatabase("AuthDB");
        });
        
        builder.Services.AddDbContext<ApplicationIdentityDatabaseContext>(options =>
        {
            options.UseInMemoryDatabase("AuthIdentityDB");
        });

        DiContainer container = new DiContainer(builder.Services);
        container.RegisterServices();
        
        #region JWT Configuration

        //======================================================
        // JWT AUTHENTICATION SETUP
        //======================================================
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT_KEY is not set in the environment variables.");
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });
        
        builder.Services.AddAuthorization();
        #endregion

        #region Identity Setup

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDatabaseContext>()
            .AddDefaultTokenProviders();

        #endregion

        var app = builder.Build();

        #region Role Initialization

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await InitializeRoles(services);
        }

        #endregion

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
    
    private static async Task InitializeRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}