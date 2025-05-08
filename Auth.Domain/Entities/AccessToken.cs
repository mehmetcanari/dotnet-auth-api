namespace Auth.Domain.Entities;

public class AccessToken
{
    public required string Token { get; init; }
    public DateTime Expires { get; init; }
}