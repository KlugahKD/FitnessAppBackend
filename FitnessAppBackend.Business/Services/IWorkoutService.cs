using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IWorkoutService
{
    Task<ServiceResponse<WorkoutPlan>> CreateWorkoutPlanAsync(WorkoutPlanRequest plan);
    Task<ServiceResponse<WorkoutPlan>> UpdateWorkoutPlanAsync(string planId,  WorkoutPlanRequest updateRequest);
    Task<ServiceResponse<bool>> DeleteWorkoutPlanAsync(string planId, string userId);
    Task<ServiceResponse<Exercise>> GetExerciseWithStepsAsync(string exerciseId, string userId);
    Task<ServiceResponse<PagedResult<Exercise>>> GetPaginatedExercisesAsync(string userId, BaseFilter filter);
    Task<int> GetTotalWorkoutsAsync(string userId);
    Task<WeeklyStatsDto> GetWeeklyWorkoutStatsAsync(string userId);
    Task<int> CalculateWorkoutStreakAsync(string userId);
    Task<GraphDataDto> GetWorkoutGraphDataAsync(string userId);
}