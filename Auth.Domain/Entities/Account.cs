namespace Auth.Domain.Entities;
public class Account
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}