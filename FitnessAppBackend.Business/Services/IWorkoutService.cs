using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IWorkoutService
{
    Task<ServiceResponse<WorkoutPlan>> CreateWorkoutPlanAsync(string userId, WorkoutPlanRequest plan);
    Task<WorkoutPlan?> GetWorkoutPlanAsync(int planId, string userId);
    Task<IEnumerable<WorkoutPlan>> GetUserWorkoutPlansAsync(string userId);
    Task<WorkoutPlan> UpdateWorkoutPlanAsync(WorkoutPlan plan);
    Task DeleteWorkoutPlanAsync(int planId, string userId);
}