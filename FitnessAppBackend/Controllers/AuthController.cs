using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

/// <summary>
/// Controller for handling authentication-related actions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">The registration model containing user details.</param>
    /// <returns>An IActionResult indicating the result of the registration.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var response = await authService.RegisterAsync(model);
        
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="model">The login model containing user credentials.</param>
    /// <returns>An IActionResult indicating the result of the login.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var response = await authService.LoginAsync(model);
        return ActionResultHelper.ToActionResult(response);
    }
}