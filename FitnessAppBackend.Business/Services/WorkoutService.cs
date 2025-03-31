using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Mapster; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class WorkoutService(ApplicationDbContext context, ILogger<WorkoutService> logger) : IWorkoutService
{
    public async Task<ServiceResponse<WorkoutPlan>> CreateWorkoutPlanAsync(string userId, WorkoutPlanRequest plan)
    {
        try
        {
            logger.LogInformation("Creating workout plan");

            if (!await UserExistsAsync(userId)) 
            {
                logger.LogDebug("User not found");

                return ResponseHelper.NotFoundResponse<WorkoutPlan>("User not found");
            }

            var planExists =
                await context.WorkoutPlans.AnyAsync(
                    p => p.UserId == userId && p.Description == plan.PlanType.ToString());
            if (planExists)
            {
                logger.LogDebug("Workout plan already exists");

                return ResponseHelper.BadRequestResponse<WorkoutPlan>("Workout plan already exists");
            }
            
            var workoutPlan = new WorkoutPlan()
            {
                Id = Guid.NewGuid().ToString("N"),
                UserId = userId,
                Description = plan.PlanType.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Exercises = GenerateExercises(plan)
            };

            context.WorkoutPlans.Add(workoutPlan);
            await context.SaveChangesAsync();
            return ResponseHelper.OkResponse(workoutPlan.Adapt<WorkoutPlan>());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating workout plan");

            return ResponseHelper.InternalServerErrorResponse<WorkoutPlan>("Error creating workout plan");
        }
    }

    public async Task<ServiceResponse<WorkoutPlan>> GetWorkoutPlanAsync(string planId, string userId)
    {
        try
        {
            logger.LogInformation("Fetching workout plan with ID: {PlanId}", planId);
            
            if (!await UserExistsAsync(userId)) 
            {
                logger.LogDebug("User not found");

                return ResponseHelper.NotFoundResponse<WorkoutPlan>("User not found");
            }


            var planExists = await context.WorkoutPlans.AnyAsync(p => p.Id == planId && p.UserId == userId);
            if (!planExists)
            {
                logger.LogDebug("Workout plan not found");

                return ResponseHelper.NotFoundResponse<WorkoutPlan>("Workout plan not found");
            }

            var plan = await context.WorkoutPlans
                .Include(p => p.Exercises)
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);

            if (plan != null) return ResponseHelper.OkResponse(plan.Adapt<WorkoutPlan>()); 
            logger.LogDebug("User has no workout plan");

            return ResponseHelper.NotFoundResponse<WorkoutPlan>("User has no workout plan");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ServiceResponse<PagedResult<WorkoutPlan>>> GetUserWorkoutPlansAsync(string userId, BaseFilter filter)
    {
        try
        {
            logger.LogInformation("Fetching all workout plans for user: {UserId}", userId);
            
            if (!await UserExistsAsync(userId)) 
            {
                logger.LogDebug("User not found");

                return ResponseHelper.NotFoundResponse<PagedResult<WorkoutPlan>>("User not found");
            }
            
            var plans = context.WorkoutPlans
                .Include(p => p.Exercises)
                .Where(p => p.UserId == userId)
                .AsNoTracking().AsQueryable();
            
            
            var totalCount = await plans.CountAsync();
            
            var data = await plans
                .OrderByDescending(t => t.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new WorkoutPlan
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    User = p.User,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Exercises = p.Exercises,

                }).ToListAsync();
            
            var response = new PagedResult<WorkoutPlan>
            {
                TotalCount = totalCount,
                PageSize = filter.PageSize,
                Page = filter.PageNumber,
                Payload = data
            };
            
            return ResponseHelper.OkResponse(response);
        }
        catch (Exception e)
        {
          logger.LogError(e, "Error fetching workout plans");
            return ResponseHelper.InternalServerErrorResponse<PagedResult<WorkoutPlan>>("Error fetching workout plans");
        }
    }

    public async Task<ServiceResponse<WorkoutPlan>> UpdateWorkoutPlanAsync(string planId, WorkoutPlanRequest updateRequest)
    {
        try
        {
            logger.LogInformation("Updating workout plan with ID: {PlanId}", planId);
            
            var planExists = await context.WorkoutPlans.AnyAsync(p => p.Id == planId);
            if (!planExists)
            {
                logger.LogDebug("Workout plan not found");

                return ResponseHelper.NotFoundResponse<WorkoutPlan>("Workout plan not found");
            }
            
            if (!await UserExistsAsync(updateRequest.UserId)) 
            {
                logger.LogDebug("User not found");

                return ResponseHelper.NotFoundResponse<WorkoutPlan>("User not found");
            }
            
            var plan = await context.WorkoutPlans
                .Include(p => p.Exercises)
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == updateRequest.UserId);
            
            if (plan == null)
            {
                logger.LogDebug("plan does not belong to user");

                return ResponseHelper.NotFoundResponse<WorkoutPlan>("plan does not belong to user");
            }
            
            plan.UpdatedAt = DateTime.UtcNow;
            plan.Exercises = GenerateExercises(updateRequest);
            plan.Description = updateRequest.PlanType.ToString();
            context.WorkoutPlans.Update(plan);
            var isSaved = await context.SaveChangesAsync() > 0;
            if (!isSaved)
            {
              logger.LogDebug("Could not update workout plan");
              
                return ResponseHelper.InternalServerErrorResponse<WorkoutPlan>("Could not update workout plan");
            }
            return ResponseHelper.OkResponse(plan.Adapt<WorkoutPlan>());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching workout plan");  
            
            return ResponseHelper.InternalServerErrorResponse<WorkoutPlan>("Error fetching workout plan");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteWorkoutPlanAsync(string planId, string userId)
    {
        try
        {
            logger.LogInformation("Deleting workout plan with ID: {PlanId}", planId);
            
            if (!await UserExistsAsync(userId)) 
            {
                logger.LogDebug("User not found");
                
                return ResponseHelper.NotFoundResponse<bool>("User not found");
            }
            
            var planExists = await context.WorkoutPlans.AnyAsync(p => p.Id == planId && p.UserId == userId);
            if (!planExists)
            {
                logger.LogDebug("Workout plan not found");

                return ResponseHelper.NotFoundResponse<bool>("Workout plan not found");
            }
            
            var plan = await context.WorkoutPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);

            if (plan != null)
            {
                logger.LogInformation("Deleting workout plan with ID: {PlanId}", planId);
                context.WorkoutPlans.Remove(plan);
                await context.SaveChangesAsync();
            }
            
            return ResponseHelper.OkResponse(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting workout plan");
            
            return ResponseHelper.InternalServerErrorResponse<bool>("Error deleting workout plan");
        }
    }


    private static List<Exercise> GenerateExercises(WorkoutPlanRequest plan)
    {
        var exercises = new List<Exercise>();

        switch (plan.PlanType)
        {
            case WorkoutPlanType.LoseWeight:
                exercises.Add(new Exercise { Name = "Running", DurationMinutes = 30, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Cycling", DurationMinutes = 45, UserId = plan.UserId});
                break;
            case WorkoutPlanType.GainMuscle:
                exercises.Add(new Exercise { Name = "Weight Lifting", DurationMinutes = 60, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Push Ups", DurationMinutes = 20, UserId = plan.UserId});
                break;
            case WorkoutPlanType.ImproveEndurance:
                exercises.Add(new Exercise { Name = "Swimming", DurationMinutes = 30, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Jogging", DurationMinutes = 40, UserId = plan.UserId});
                break;
            case WorkoutPlanType.IncreaseFlexibility:
                exercises.Add(new Exercise { Name = "Yoga", DurationMinutes = 60, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Stretching", DurationMinutes = 30, UserId = plan.UserId});
                break;
            case WorkoutPlanType.BuildStrength:
                exercises.Add(new Exercise { Name = "Deadlifts", DurationMinutes = 45, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Squats", DurationMinutes = 30, UserId = plan.UserId});
                break;
            case WorkoutPlanType.CardioFitness:
                exercises.Add(new Exercise { Name = "Jump Rope", DurationMinutes = 20, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "HIIT", DurationMinutes = 30, UserId = plan.UserId});
                break;
            case WorkoutPlanType.ToneBody:
                exercises.Add(new Exercise { Name = "Pilates", DurationMinutes = 50, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Bodyweight Exercises", DurationMinutes = 40, UserId = plan.UserId});
                break;
            case WorkoutPlanType.ImproveBalance:
                exercises.Add(new Exercise { Name = "Balance Board", DurationMinutes = 30, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Tai Chi", DurationMinutes = 45, UserId = plan.UserId});
                break;
            case WorkoutPlanType.IncreaseStamina:
                exercises.Add(new Exercise { Name = "Rowing", DurationMinutes = 40, UserId = plan.UserId});
                exercises.Add(new Exercise { Name = "Stair Climbing", DurationMinutes = 30, UserId = plan.UserId});
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return exercises;
    }
    
    private async Task<bool> UserExistsAsync(string userId)
    {
        return await context.Users.AnyAsync(u => u.Id == userId);
    }
}