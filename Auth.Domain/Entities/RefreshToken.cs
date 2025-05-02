namespace Auth.Domain.Entities;
public class RefreshToken
{
    public int Id { get; init; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public int AccountId { get; set; }

    public RefreshToken(string token, DateTime expiresAt, int accountId)
    {
        Token = token;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        AccountId = accountId;
    }
}