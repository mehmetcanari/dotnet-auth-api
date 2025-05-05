namespace Auth.Application.DTO;

public record AccountResponseDto
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}