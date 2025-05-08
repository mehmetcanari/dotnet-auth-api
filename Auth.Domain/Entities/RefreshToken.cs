namespace Auth.Domain.Entities;
public class RefreshToken
{
    public int Id { get; init; }
    public required string Token { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; set; }
    public required string Email { get; init; }

    public void Revoke()
    {
        IsRevoked = true;
    }
}