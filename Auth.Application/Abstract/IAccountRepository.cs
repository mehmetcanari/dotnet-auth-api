using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IAccountRepository
{
    Task<Account> GetAccountByIdAsync(int accountId);
    Task AddAccountAsync(Account account);
}