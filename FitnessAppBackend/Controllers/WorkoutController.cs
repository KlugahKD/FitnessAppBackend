using System.Security.Claims;
using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkoutController(IWorkoutService workoutService) : ControllerBase
{
    /// <summary>
    /// Creates a new workout plan for the authenticated user.
    /// </summary>
    /// <param name="plan">The workout plan request containing the details of the plan.</param>
    /// <returns>The created workout plan.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateWorkoutPlan(WorkoutPlanRequest plan)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await workoutService.CreateWorkoutPlanAsync(userId, plan);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Retrieves a specific workout plan by ID for the authenticated user.
    /// </summary>
    /// <param name="id">The ID of the workout plan.</param>
    /// <returns>The workout plan.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkoutPlan(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await workoutService.GetWorkoutPlanAsync(id, userId);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Retrieves all workout plans for the authenticated user.
    /// </summary>
    /// <returns>A list of workout plans.</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserWorkoutPlans([FromBody] BaseFilter filter)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await workoutService.GetUserWorkoutPlansAsync(userId, filter);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Updates a specific workout plan for the authenticated user.
    /// </summary>
    /// <param name="id">The ID of the workout plan to update.</param>
    /// <param name="plan">The updated workout plan details.</param>
    /// <returns>The updated workout plan.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkoutPlan(string id, WorkoutPlanRequest plan)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null || userId != plan.UserId) return Unauthorized();

        var response = await workoutService.UpdateWorkoutPlanAsync(id, plan);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Deletes a specific workout plan for the authenticated user.
    /// </summary>
    /// <param name="id">The ID of the workout plan to delete.</param>
    /// <returns>No content if the deletion was successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutPlan(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await workoutService.DeleteWorkoutPlanAsync(id, userId);
        return ActionResultHelper.ToActionResult(response);
    }
}