using Azure;
using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class ExerciseTrackingService(ApplicationDbContext context, ILogger<ExerciseTrackingService> logger): IExerciseTrackingService
{
    public async Task<ServiceResponse<bool>> LogExerciseAsync(string userId, string exerciseName, int duration, int caloriesBurned)
    {
        try
        {
            logger.LogInformation("Exercise tracking started");
            
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                logger.LogDebug("User {UserId} not found.", userId);

                return ResponseHelper.NotFoundResponse<bool>("User not found");
            }

            var exerciseLog = new ExerciseLog
            {
                UserId = userId,
                ExerciseName = exerciseName,
                DurationMinutes = duration,
                CaloriesBurned = caloriesBurned,
            };

            context.ExerciseLogs.Add(exerciseLog);
            await context.SaveChangesAsync();

            return ResponseHelper.OkResponse(true);
        }
        catch (Exception e)
        {
           logger.LogError(e, "Log exercise failed");
           
            return ResponseHelper.InternalServerErrorResponse<bool>("An error occurred while logging the exercise");
        }
    }

    public async Task<ServiceResponse<ExerciseLog>> GetUserExercisesAsync(string userId)
    {
        try
        {
            logger.LogInformation("Fetching user exercises for {UserId}", userId);

            var exercises = await context.ExerciseLogs
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.LoggedAt)
                .ToListAsync();

            return ResponseHelper.OkResponse(exercises.Adapt<ExerciseLog>());
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetUserExercises failed");

            return ResponseHelper.InternalServerErrorResponse<ExerciseLog>(
                "An error occurred while retrieving user exercises");
        }
    }
}