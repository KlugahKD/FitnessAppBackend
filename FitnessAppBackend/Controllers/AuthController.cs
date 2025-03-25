using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var response = await authService.RegisterAsync(model);
        
        return ActionResultHelper.ToActionResult(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var response = await authService.LoginAsync(model);
        
        return ActionResultHelper.ToActionResult(response);
    }
}