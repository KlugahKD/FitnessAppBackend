using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppBackend.Business.Services;

public class HealthAdviceService(ApplicationDbContext context) : IHealthAdviceService
{
    public async Task<IEnumerable<HealthAdvice>> GetPersonalizedAdviceAsync(string userId)
    {
        var user = await context.Users
            .Include(u => u.WorkoutPlans)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        // Generate personalized advice based on user's workout history and goals
        var advice = new List<HealthAdvice>();
        
        if (user.WorkoutPlans.Any())
        {
            var recentWorkouts = user.WorkoutPlans.OrderByDescending(w => w.CreatedAt).Take(5);
            foreach (var workout in recentWorkouts)
            {
                advice.Add(new HealthAdvice
                {
                    Title = "Workout Progress",
                    Content = $"Great progress on {workout.Name}! Keep up the momentum.",
                    Category = "Progress",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        return advice;
    }

    public async Task<HealthAdvice> AddHealthAdviceAsync(HealthAdvice advice)
    {
        advice.CreatedAt = DateTime.UtcNow;
        context.HealthAdvice.Add(advice);
        await context.SaveChangesAsync();
        return advice;
    }

    public async Task<IEnumerable<HealthAdvice>> GetDailyTipsAsync()
    {
        return await context.HealthAdvice
            .Where(a => a.Category == "DailyTip")
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
}