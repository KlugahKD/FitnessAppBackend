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
    /// Retrieves a exercises to be done for the day.
    /// </summary>
    [HttpGet("exercises/today")]
    public async Task<IActionResult> GetPaginatedExercises([FromQuery] string userId)
    {
        var response = await workoutService.GetExercisesForTodayAsync(userId);
        
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
    
    [HttpGet("exercises/summary")]
    public async Task<IActionResult> GetCompletedAndMissedWorkouts([FromQuery] string userId)
    {
        var response = await workoutService.GetCompletedAndMissedWorkoutsAsync(userId);
        return ActionResultHelper.ToActionResult(response);
    }
    
    [HttpPut("exercises/{exerciseId}/complete")]
    public async Task<IActionResult> MarkExerciseAsCompleted(string exerciseId, [FromQuery] string userId)
    {
        var response = await workoutService.MarkExerciseAsCompletedAsync(exerciseId, userId);
        
        return ActionResultHelper.ToActionResult(response);
    }
    
    [HttpPut("steps/{stepId}/complete")]
    public async Task<IActionResult> MarkStepAsCompleted(string stepId, [FromQuery] string userId)
    {
        var response = await workoutService.MarkStepAsCompletedAsync(stepId, userId);
        
        return ActionResultHelper.ToActionResult(response);
    }
}