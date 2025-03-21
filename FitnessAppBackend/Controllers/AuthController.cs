using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterModel model)
    {
        try
        {
            var response = await authService.RegisterAsync(model);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginModel model)
    {
        try
        {
            var response = await authService.LoginAsync(model);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}