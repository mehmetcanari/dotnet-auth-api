using Auth.Application.Abstract;
using Auth.Application.DTO;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAccountService _accountService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthService> _logger;
    private const string UserRole = "User";
    
    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, 
        IAccountService accountService, 
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _accountService = accountService;
        _logger = logger;
    }

    public async Task<string> RegisterAsync(AccountRegisterRequestDto accountRegisterRequestDto)
    {
        try
        {
            IdentityUser user = new IdentityUser
            {
                UserName = accountRegisterRequestDto.Name,
                Email = accountRegisterRequestDto.Email,
                EmailConfirmed = true
            };
        
            var result = await _userManager.CreateAsync(user, accountRegisterRequestDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRole);
                await _accountService.RegisterAccountAsync(accountRegisterRequestDto);
                return "User registered successfully";
            }

            _logger.LogError("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return "User registration failed";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while registering the user: {Email}", accountRegisterRequestDto.Email);
            throw;
        }
    }

    public async Task<AuthLoginResponseDto> LoginAsync(AccountLoginRequestDto accountLoginRequestDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(accountLoginRequestDto.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _signInManager.PasswordSignInAsync(user, accountLoginRequestDto.Password, false, false);
            if (!result.Succeeded) throw new Exception("Invalid login attempt");
            
            var accessToken = await _accessTokenService.GenerateAccessTokenAsync(accountLoginRequestDto.Email);
            await _refreshTokenService.GenerateRefreshTokenAsync(accountLoginRequestDto.Email);

            AuthLoginResponseDto authLoginResponseDto = new()
            {
                Token = accessToken.Token,
                Expire = accessToken.Expires,
            };
            
            return authLoginResponseDto;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while logging in the user: {Email}", accountLoginRequestDto.Email);
            throw;
        }
    }
}