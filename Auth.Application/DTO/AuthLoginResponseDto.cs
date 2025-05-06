namespace Auth.Application.DTO;

public record AuthLoginResponseDto
{
    public required string Token { get; init; }
    public required string Expire { get; init; }
}