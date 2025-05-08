using System.Security.Claims;
using Auth.Application.Abstract;
using Auth.Application.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.WebAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var account = await _accountService.GetAccountByEmailAsync(email);

                return Ok(account);
            }
            
            return StatusCode(404, ServiceResult.Failure("Error", "Account not found"));
        }
        catch (Exception e)
        {
            return StatusCode(404, ServiceResult.Failure("Error", e.Message));
        }
    }
}