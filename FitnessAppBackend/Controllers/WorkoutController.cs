using System.Security.Claims;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkoutController(IWorkoutService workoutService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<WorkoutPlan>> CreateWorkoutPlan(WorkoutPlanRequest plan)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var createdPlan = await workoutService.CreateWorkoutPlanAsync(userId, plan);
        return CreatedAtAction(nameof(GetWorkoutPlan), new { id = createdPlan.Id }, createdPlan);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutPlan>> GetWorkoutPlan(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var plan = await workoutService.GetWorkoutPlanAsync(id, userId);
        if (plan == null) return NotFound();

        return plan;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetUserWorkoutPlans()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var plans = await workoutService.GetUserWorkoutPlansAsync(userId);
        return Ok(plans);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WorkoutPlan>> UpdateWorkoutPlan(int id, WorkoutPlan plan)
    {
        if (id != plan.Id) return BadRequest();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null || userId != plan.UserId) return Unauthorized();

        var updatedPlan = await workoutService.UpdateWorkoutPlanAsync(plan);
        return Ok(updatedPlan);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutPlan(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        await workoutService.DeleteWorkoutPlanAsync(id, userId);
        return NoContent();
    }
}