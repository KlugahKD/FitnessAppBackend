using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IWorkoutService
{
    Task<ServiceResponse<WorkoutPlan>> CreateWorkoutPlanAsync(string userId, WorkoutPlanRequest plan);
    Task<ServiceResponse<WorkoutPlan>> GetWorkoutPlanAsync(int planId, string userId);
    Task<ServiceResponse<PagedResult<WorkoutPlan>>> GetUserWorkoutPlansAsync(string userId, BaseFilter filter);
    Task<ServiceResponse<WorkoutPlan>> UpdateWorkoutPlanAsync(int planId,  WorkoutPlanRequest updateRequest);
    Task<ServiceResponse<bool>> DeleteWorkoutPlanAsync(int planId, string userId);
}