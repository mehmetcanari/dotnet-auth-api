using System.Security.Claims;
using Auth.Application.Abstract;
using Auth.Application.Common.Responses;
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger,
        IRefreshTokenService refreshTokenService, IAccessTokenService accessTokenService)
    {
        _authService = authService;
        _logger = logger;
        _refreshTokenService = refreshTokenService;
        _accessTokenService = accessTokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequestDto accountRegisterRequestDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(accountRegisterRequestDto);
            
            if (result)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return StatusCode(400, ServiceResult.Failure("RegistrationFailed", "User registration failed."));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while registering the user: {Email}",
                accountRegisterRequestDto.Email);
            return StatusCode(400, ServiceResult.Failure("Error", e.Message));
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AccountLoginRequestDto accountLoginRequestDto)
    {
        try
        {
            var authResult = await _authService.LoginAsync(accountLoginRequestDto);
            return Ok(authResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while logging in the user: {Email}", accountLoginRequestDto.Email);
            return StatusCode(400, ServiceResult.Failure("Error", e.Message));
        }
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(ServiceResult.Failure("InvalidToken", "User email is missing."));
            }

            await _authService.LogoutAsync(User);
            return Ok(new { message = "User logged out successfully." });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while logging out the user.");
            return StatusCode(400, ServiceResult.Failure("Error", e.Message));
        }
    }
    

    [AllowAnonymous]
    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var refreshTokenFromCookie = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshTokenFromCookie))
            {
                return Unauthorized(ServiceResult.Failure("InvalidToken", "Refresh token is missing."));
            }
            
            var email = await _refreshTokenService.ValidateRefreshTokenAsync(refreshTokenFromCookie);

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(ServiceResult.Failure("InvalidToken", "Refresh token is invalid."));
            }

            await _refreshTokenService.GenerateRefreshTokenAsync(email);
            var accessToken = await _accessTokenService.GenerateAccessTokenAsync(email);

            return Ok(accessToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while refreshing the token.");
            return StatusCode(400, ServiceResult.Failure("Error", e.Message));
        }
    }
}