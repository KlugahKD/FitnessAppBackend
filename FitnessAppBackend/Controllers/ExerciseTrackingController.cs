using System.Security.Claims;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

/// <summary>
/// Controller for handling exercise tracking-related actions.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExerciseTrackingController(IExerciseTrackingService exerciseTrackingService) : ControllerBase
{
    /// <summary>
    /// Logs an exercise for the authenticated user.
    /// </summary>
    /// <param name="logRequest">The exercise log request containing exercise details.</param>
    /// <returns>An IActionResult indicating the result of the logging operation.</returns>
    [HttpPost("log")]
    public async Task<IActionResult> LogExercise([FromBody] ExerciseLogRequest logRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await exerciseTrackingService.LogExerciseAsync(userId, logRequest.ExerciseName, logRequest.Duration, logRequest.CaloriesBurned);
        return ActionResultHelper.ToActionResult(response);
    }

    /// <summary>
    /// Retrieves all exercise logs for the authenticated user.
    /// </summary>
    /// <returns>A list of exercise logs.</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserExercises()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var response = await exerciseTrackingService.GetUserExercisesAsync(userId);
        return ActionResultHelper.ToActionResult(response);
    }
}