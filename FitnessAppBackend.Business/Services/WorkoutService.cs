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
            
            var userExists = await context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                logger.LogDebug("User not found");
                
                return ResponseHelper.NotFoundResponse<WorkoutPlan>("User not found");
            }

            var workoutPlan = new WorkoutPlan()
            {
                Id = new Random().Next(1, 1000000),
                UserId = userId,
                Description = plan.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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

    public async Task<WorkoutPlan?> GetWorkoutPlanAsync(int planId, string userId)
    {
        return await context.WorkoutPlans
            .Include(p => p.Exercises)
            .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);
    }

    public async Task<IEnumerable<WorkoutPlan>> GetUserWorkoutPlansAsync(string userId)
    {
        return await context.WorkoutPlans
            .Include(p => p.Exercises)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<WorkoutPlan> UpdateWorkoutPlanAsync(WorkoutPlan plan)
    {
        plan.UpdatedAt = DateTime.UtcNow;
        context.WorkoutPlans.Update(plan);
        await context.SaveChangesAsync();
        return plan;
    }

    public async Task DeleteWorkoutPlanAsync(int planId, string userId)
    {
        var plan = await context.WorkoutPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);
        
        if (plan != null)
        {
            context.WorkoutPlans.Remove(plan);
            await context.SaveChangesAsync();
        }
    }
}