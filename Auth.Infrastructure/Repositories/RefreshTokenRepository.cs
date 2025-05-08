using Auth.Domain.Entities;
using Auth.Application.Abstract;
using Auth.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDatabaseContext _context;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(ApplicationDatabaseContext context, ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<RefreshToken>> GetActiveRefreshTokensAsync(string email)
    {
        try
        {
            List<RefreshToken> refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt > DateTime.UtcNow 
                             && rt.IsRevoked == false 
                             && rt.Email == email)
                .AsNoTracking()
                .ToListAsync();
            
            return refreshTokens;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving active refresh tokens.");
            throw;
        }
    }

    public async Task RemoveRefreshTokensAsync(string email)
    {
        try
        {
            var refreshTokens = await GetActiveRefreshTokensAsync(email);
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while removing the refresh token.");
            throw;
        }
    }
    
    public async Task<RefreshToken> GetRefreshTokenByTokenAsync(string token)
    {
        try
        {
            var refreshToken = await _context.RefreshTokens
                .Where(rt => rt.Token == token && rt.IsRevoked == false)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (refreshToken == null)
            {
                throw new KeyNotFoundException($"Refresh token {token} not found.");
            }
            
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the refresh token {Token}", token);
            throw;
        }
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the refresh token.");
            throw;
        }
    }
}