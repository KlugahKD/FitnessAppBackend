using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppBackend.Business.Services;

public class WorkoutService(ApplicationDbContext context) : IWorkoutService
{
    public async Task<WorkoutPlan> CreateWorkoutPlanAsync(string userId, WorkoutPlan plan)
    {
        plan.UserId = userId;
        plan.CreatedAt = DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;

        context.WorkoutPlans.Add(plan);
        await context.SaveChangesAsync();
        return plan;
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