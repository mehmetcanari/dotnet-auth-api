using Auth.Domain.Entities;
using Auth.Application.Abstract;
using Auth.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDatabaseContext _context;

    public RefreshTokenRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(int accountId)
    {
        try
        {
            IQueryable<RefreshToken> query = _context.RefreshTokens;

            var refreshToken = await query.Where(a => a.AccountId == accountId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (refreshToken == null)
            {
                throw new KeyNotFoundException($"Refresh token for account ID {accountId} not found.");
            }

            return refreshToken;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the refresh token.", ex);
        }
    }

    public async Task<List<RefreshToken>> GetActiveRefreshTokensAsync()
    {
        try
        {
            List<RefreshToken> refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt > DateTime.UtcNow && rt.IsRevoked == false)
                .AsNoTracking()
                .ToListAsync();
            
            return refreshTokens;
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while retrieving active refresh tokens.", e);
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
            throw new Exception("An error occurred while adding the refresh token.", ex);
        }
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the refresh token.", ex);
        }
    }
}