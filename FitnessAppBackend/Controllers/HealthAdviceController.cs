using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthAdviceController(IHealthAdviceService healthAdviceService) : ControllerBase
{
    [HttpGet("personalized")]
    public async Task<ActionResult<IEnumerable<HealthAdvice>>> GetPersonalizedAdvice([FromQuery] string userId)
    {
        var advice = await healthAdviceService.GetPersonalisedHealthAdvice(userId);
        return Ok(advice);
    }
}