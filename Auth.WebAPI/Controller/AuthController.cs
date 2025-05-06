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
    private readonly IAccountService _accountService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IAccountService accountService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _accountService = accountService;
        _logger = logger;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequestDto accountRegisterRequestDto)
    {
        try
        {
            var authResult = await _authService.RegisterAsync(accountRegisterRequestDto);
            var accountResult = await _accountService.RegisterAccountAsync(accountRegisterRequestDto);
        
            if (authResult && accountResult)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return BadRequest(ServiceResult.Failure("RegistrationFailed", "User registration failed."));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while registering the user: {Email}", accountRegisterRequestDto.Email);
            return BadRequest(ServiceResult.Failure("RegistrationFailed", "User registration failed."));
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
            return BadRequest(ServiceResult.Failure("LoginFailed", "User login failed."));
        }
    }
}