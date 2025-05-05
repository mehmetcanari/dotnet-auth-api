namespace Auth.Domain.Entities;

public class AccessToken
{
    public required string Token { get; set; }
    public DateTime Expires { get; set; }
}