using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

/// <summary>
/// Controller for handling avatar-related actions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AvatarController(IAvatarService avatarService) : ControllerBase
{
    /// <summary>
    /// Gets a response from the user's preferred avatar based on the provided question.
    /// </summary>
    /// <param name="userId">The ID of the user interacting with the avatar.</param>
    /// <param name="question">The question to ask the avatar.</param>
    /// <returns>A response from the avatar.</returns>
    [HttpGet("response")]
    public async Task<IActionResult> GetResponse([FromQuery] string userId, [FromQuery] string question)
    {
        var result = await avatarService.GetResponseAsync(userId, question);

        return ActionResultHelper.ToActionResult(result);
    }

    /// <summary>
    /// Gets a motivational message from the user's preferred avatar.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the motivational message.</param>
    /// <returns>A motivational message from the avatar.</returns>
    [HttpGet("motivational-message")]
    public async Task<IActionResult> GetMotivationalMessage([FromQuery] string userId)
    {
        var result = await avatarService.GetMotivationalMessageAsync(userId);
        return ActionResultHelper.ToActionResult(result);
    }
}