using System.Security.Claims;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AvatarController(IAvatarService avatarService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Avatar>>> GetAvailableAvatars()
    {
        var avatars = await avatarService.GetAvailableAvatarsAsync();
        return Ok(avatars);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Avatar>> GetAvatar(int id)
    {
        try
        {
            var avatar = await avatarService.GetAvatarByIdAsync(id);
            return Ok(avatar);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("assign/{avatarId}")]
    public async Task<ActionResult<Avatar>> AssignAvatar(int avatarId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        try
        {
            var avatar = await avatarService.AssignAvatarToUserAsync(userId, avatarId);
            return Ok(avatar);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}