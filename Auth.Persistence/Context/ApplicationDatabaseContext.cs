using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence.Context;

public class ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}