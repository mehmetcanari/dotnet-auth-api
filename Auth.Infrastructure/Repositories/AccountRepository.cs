using Auth.Application.Abstract;
using Auth.Domain.Entities;
using Auth.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDatabaseContext _context;

    public AccountRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Account> GetAccountByIdAsync(int accountId)
    {
        try
        {
            IQueryable<Account> query = _context.Accounts;

            var account = await query.Where(a => a.Id == accountId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            }
            return account;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the account.", ex);
        }
    }
    
    public async Task<Account> GetAccountByEmailAsync(string email)
    {
        try
        {
            IQueryable<Account> query = _context.Accounts;

            var account = await query.Where(a => a.Email == email)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (account == null)
            {
                throw new KeyNotFoundException($"Account with email {email} not found.");
            }
            return account;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the account.", ex);
        }
    }

    public async Task AddAccountAsync(Account account)
    {
        try
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the account.", ex);
        }
    }
    
    public async Task RemoveAccountAsync(Account account)
    {
        try
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while removing the account.", ex);
        }
    }
}