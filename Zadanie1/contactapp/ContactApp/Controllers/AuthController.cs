using Microsoft.AspNetCore.Mvc;
using ContactApp.DTOs;
using ContactApp.Services;

namespace ContactApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/login . login endpoint - returns JWT token
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);

        if (token == null)
            return Unauthorized(new { message = "Invalid" });

        return Ok(new { token });
    }
}
