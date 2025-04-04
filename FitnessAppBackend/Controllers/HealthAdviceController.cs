using System.Security.Claims;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HealthAdviceController(IHealthAdviceService healthAdviceService) : ControllerBase
{
    [HttpGet("personalized")]
    public async Task<ActionResult<IEnumerable<HealthAdvice>>> GetPersonalizedAdvice()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        try
        {
            var advice = await healthAdviceService.GetPersonalizedAdviceAsync(userId);
            return Ok(advice);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("daily-tips")]
    public async Task<ActionResult<IEnumerable<HealthAdvice>>> GetDailyTips()
    {
        var tips = await healthAdviceService.GetDailyTipsAsync();
        return Ok(tips);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HealthAdvice>> AddHealthAdvice(HealthAdvice advice)
    {
        var newAdvice = await healthAdviceService.AddHealthAdviceAsync(advice);
        return CreatedAtAction(nameof(GetDailyTips), null, newAdvice);
    }
}