namespace Auth.Application.DTO;

public record AccountLoginRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}