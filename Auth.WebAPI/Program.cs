using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Persistence.Context;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth.WebAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        Env.Load();        
        
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_SECRET");
        builder.Configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
        builder.Configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddDbContext<ApplicationDatabaseContext>(options =>
        {
            options.UseInMemoryDatabase("AuthDB");
        });

        DiContainer container = new DiContainer(builder.Services);
        container.RegisterServices();
        
        #region JWT Authentication

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
        
        #endregion

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}