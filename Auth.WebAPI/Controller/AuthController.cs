using Auth.Application.Abstract;
using Auth.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.WebAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAccessTokenService _accessTokenService;

    public AuthController(
        IAuthService authService,
        IRefreshTokenService refreshTokenService, 
        IAccessTokenService accessTokenService)
    {
        _authService = authService;
        _refreshTokenService = refreshTokenService;
        _accessTokenService = accessTokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequestDto accountRegisterRequestDto)
    {
        await _authService.RegisterAsync(accountRegisterRequestDto);
        return Ok(new { message = "User registered successfully." });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AccountLoginRequestDto accountLoginRequestDto)
    {
        var loginResponse = await _authService.LoginAsync(accountLoginRequestDto);
        return Ok( new { accessToken = loginResponse.Token });
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { message = "User logged out successfully." });
    }
    

    [AllowAnonymous]
    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        await _refreshTokenService.GenerateRefreshTokenAsync();
        var accessToken = await _accessTokenService.GenerateAccessTokenAsync();

        return Ok(accessToken);
    }
}