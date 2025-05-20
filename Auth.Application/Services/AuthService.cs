using System.Security.Claims;
using Auth.Application.Abstract;
using Auth.Application.DTO;
using Auth.Application.Utility;
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
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthService> _logger;
    private const string DefaultRole = "User";

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService,
        IAccountService accountService,
        ILogger<AuthService> logger, 
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _accountService = accountService;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task RegisterAsync(AccountRegisterRequestDto accountRegisterRequestDto)
    {
        try
        {
            IdentityUser user = new IdentityUser
            {
                UserName = accountRegisterRequestDto.Name,
                Email = accountRegisterRequestDto.Email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(user, accountRegisterRequestDto.Password);
            await _userManager.AddToRoleAsync(user, DefaultRole);
            await _accountService.RegisterAccountAsync(accountRegisterRequestDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while registering the user: {Email}", accountRegisterRequestDto.Email);
            throw new Exception("An error occurred while registering the user.", e);
        }
    }

    public async Task<AuthLoginResponseDto> LoginAsync(AccountLoginRequestDto accountLoginRequestDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(accountLoginRequestDto.Email);
            if (user == null)
            {
                _logger.LogError("User with email {Email} not found.", accountLoginRequestDto.Email);
                throw new Exception("User with email {Email} not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, accountLoginRequestDto.Password, false, false);
            if (!result.Succeeded)
            {
                _logger.LogError("Invalid login attempt for user with email {Email}.", accountLoginRequestDto.Email);
                throw new Exception("Invalid login attempt.");
            }

            var accessToken = await _accessTokenService.GenerateAccessTokenAsync();
            await _refreshTokenService.GenerateRefreshTokenAsync();
            
            AuthLoginResponseDto authLoginResponseDto = new()
            {
                Token = accessToken.Token,
                Expire = UtcToLocalConverter.ConvertToLocalTime(accessToken.Expires),
            };

            return authLoginResponseDto;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred during login.");
            throw;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var email = _currentUserService.GetCurrentUserEmail();
            if (email.Data == null)
            {
                _logger.LogError("User principal is null.");
                throw new Exception("User principal is null.");
            }

            var user = await _userManager.FindByEmailAsync(email.Data);
            if (user == null)
            {
                _logger.LogError("User with email {Email} not found.", email.Data);
                throw new Exception("User with email {Email} not found.");
            }
                
            await _signInManager.SignOutAsync();
            await _refreshTokenService.RemoveRefreshTokenAsync(email.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while logging out the user: {Email}", e.Message);
            throw;
        }
    }
}