using System.Security.Claims;
using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

/// <summary>
/// Controller for handling avatar-related actions.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AvatarController(IAvatarService avatarService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of available avatars.
    /// </summary>
    /// <param name="filter">The filter parameters for pagination.</param>
    /// <returns>A paginated list of avatars.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAvailableAvatars([FromQuery] BaseFilter filter)
    {
        var response = await avatarService.GetAvailableAvatarsAsync(filter);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Retrieves a specific avatar by ID.
    /// </summary>
    /// <param name="id">The ID of the avatar.</param>
    /// <returns>The avatar details.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAvatarById(string id)
    {
        var response = await avatarService.GetAvatarByIdAsync(id);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Assigns an avatar to the authenticated user.
    /// </summary>
    /// <param name="avatarId">The ID of the avatar to assign.</param>
    /// <returns>A boolean indicating the result of the assignment.</returns>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignAvatarToUser([FromBody] string avatarId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await avatarService.AssignAvatarToUserAsync(userId, avatarId);
        return ActionResultHelper.ToActionResult(response);
    }
}