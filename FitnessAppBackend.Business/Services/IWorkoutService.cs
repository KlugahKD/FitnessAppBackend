using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IWorkoutService
{
    Task<WorkoutPlan> CreateWorkoutPlanAsync(string userId, WorkoutPlan plan);
    Task<WorkoutPlan?> GetWorkoutPlanAsync(int planId, string userId);
    Task<IEnumerable<WorkoutPlan>> GetUserWorkoutPlansAsync(string userId);
    Task<WorkoutPlan> UpdateWorkoutPlanAsync(WorkoutPlan plan);
    Task DeleteWorkoutPlanAsync(int planId, string userId);
}