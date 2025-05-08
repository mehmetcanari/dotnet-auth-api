namespace Auth.Domain.Entities;
public class Account
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
    public DateTime CreatedAt { get; init; }
}