using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IAccountRepository
{
    Task<Account> GetAccountByIdAsync(int accountId);
    Task<Account> GetAccountByEmailAsync(string email);
    Task AddAccountAsync(Account account);
    Task RemoveAccountAsync(Account account);
}