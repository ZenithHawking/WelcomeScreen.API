using Microsoft.AspNetCore.Mvc;
using WelcomeScreen.API.Services;

namespace WelcomeScreen.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    //[HttpPost("setup")]
    //public async Task<IActionResult> Setup([FromBody] LoginDto dto)
    //{
    //    var user = await authService.CreateUser(dto.Username, dto.Username + "@local.com", dto.Password);
    //    return Ok(new { user.Id, user.Username });
    //}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await authService.ValidateUser(dto.Username, dto.Password);
        if (user == null) return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });
        var token = authService.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Username, user.Email, user.Role } });
    }
}

public record LoginDto(string Username, string Password);