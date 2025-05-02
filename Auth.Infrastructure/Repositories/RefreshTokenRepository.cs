using Auth.Domain.Entities;
using Auth.Application.Abstract;
using Auth.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDatabaseContext _context;

    public RefreshTokenRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetAccountToken(int accountId)
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

    public async Task AddAsync(RefreshToken refreshToken)
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

    public async Task UpdateAsync(RefreshToken refreshToken)
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