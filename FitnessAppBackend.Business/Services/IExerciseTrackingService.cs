using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IExerciseTrackingService
{
    Task<ServiceResponse<bool>> LogExerciseAsync(string userId, string exerciseName, int duration, int caloriesBurned);
    Task<ServiceResponse<ExerciseLog>> GetUserExercisesAsync(string userId);
}