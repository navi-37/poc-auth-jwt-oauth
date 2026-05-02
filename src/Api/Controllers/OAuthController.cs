using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("auth/oauth")]
public class OAuthController : ControllerBase
{
    private readonly IOAuthService _oAuthService;

    public OAuthController(IOAuthService oAuthService)
    {
        _oAuthService = oAuthService;
    }

    [HttpPost("{provider}")]
    public async Task<IActionResult> Login(string provider, [FromBody] OAuthLoginRequest request)
    {
        var result = await _oAuthService.LoginAsync(provider, request.Token);
        if (result is null)
            return Unauthorized(new { message = "Invalid or unsupported OAuth token" });

        return Ok(result);
    }
}
