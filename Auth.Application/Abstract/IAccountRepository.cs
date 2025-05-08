using Auth.Domain.Entities;

namespace Auth.Application.Abstract;

public interface IAccountRepository
{
    Task<Account> GetAccountByEmailAsync(string email);
    Task AddAccountAsync(Account account);
}