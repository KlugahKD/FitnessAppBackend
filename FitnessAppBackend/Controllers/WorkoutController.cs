using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutController(IWorkoutService workoutService) : ControllerBase
{
    /// <summary>
    /// Creates a new workout plan for the authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWorkoutPlan(WorkoutPlanRequest plan)
    {
        var response = await workoutService.CreateWorkoutPlanAsync(plan);
        
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Updates a specific workout plan for the authenticated user.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkoutPlan(string id, WorkoutPlanRequest plan)
    {
        var response = await workoutService.UpdateWorkoutPlanAsync(id, plan);
        
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Deletes a specific workout plan for the authenticated user.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutPlan(string id, [FromQuery] string userId)
    {
        var response = await workoutService.DeleteWorkoutPlanAsync(id, userId);
        
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Retrieves a paginated list of exercises for the authenticated user.
    /// </summary>
    [HttpGet("exercises/paginated")]
    public async Task<IActionResult> GetPaginatedExercises([FromQuery] string userId, [FromBody] BaseFilter filter)
    {
        var response = await workoutService.GetPaginatedExercisesAsync(userId, filter);
        
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Retrieves a specific exercise along with its steps for the authenticated user.
    /// </summary>
    [HttpGet("exercises/steps")]
    public async Task<IActionResult> GetExerciseWithSteps([FromQuery] string exerciseId, [FromQuery] string userId)
    {
        var response = await workoutService.GetExerciseWithStepsAsync(exerciseId, userId);
        
        return ActionResultHelper.ToActionResult(response);
    }
}