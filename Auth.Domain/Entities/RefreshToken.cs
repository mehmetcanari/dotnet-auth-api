namespace Auth.Domain.Entities;
public class RefreshToken
{
    public int Id { get; init; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public int AccountId { get; set; }

    public void Revoke()
    {
        IsRevoked = true;
    }
}