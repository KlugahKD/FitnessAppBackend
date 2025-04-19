using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppBackend.Business.Services;

public class HealthAdviceService(ApplicationDbContext context) : IHealthAdviceService
{
    private static readonly List<HealthAdvice> AdvicePool = new()
    {
        new HealthAdvice
        {
            Title = "Stay Hydrated",
            Content = "Drink at least 8 glasses of water daily to stay hydrated.",
            Category = "General",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Balanced Diet",
            Content = "Include a mix of proteins, carbs, and healthy fats in your meals.",
            Category = "Nutrition",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Regular Exercise",
            Content = "Aim for at least 30 minutes of physical activity daily.",
            Category = "Fitness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Sleep Well",
            Content = "Get 7-8 hours of quality sleep every night.",
            Category = "Wellness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Mental Health",
            Content = "Practice mindfulness or meditation to reduce stress.",
            Category = "Mental Health",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Stretching",
            Content = "Incorporate stretching exercises to improve flexibility and prevent injuries.",
            Category = "Fitness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Healthy Snacking",
            Content = "Choose fruits, nuts, or yogurt as healthy snack options.",
            Category = "Nutrition",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Posture Check",
            Content = "Maintain good posture to avoid back and neck pain.",
            Category = "Wellness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Take Breaks",
            Content = "Take short breaks during work to stay refreshed and focused.",
            Category = "Mental Health",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Outdoor Activities",
            Content = "Spend time outdoors to get fresh air and sunlight.",
            Category = "Wellness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Protein Intake",
            Content = "Ensure adequate protein intake to support muscle repair and growth.",
            Category = "Nutrition",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Cardio Benefits",
            Content = "Incorporate cardio exercises to improve heart health.",
            Category = "Fitness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Mindful Eating",
            Content = "Eat slowly and mindfully to improve digestion and avoid overeating.",
            Category = "Nutrition",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Strength Training",
            Content = "Add strength training to your routine to build muscle and bone density.",
            Category = "Fitness",
            CreatedAt = DateTime.UtcNow
        },
        new HealthAdvice
        {
            Title = "Limit Screen Time",
            Content = "Reduce screen time to avoid eye strain and improve sleep quality.",
            Category = "Wellness",
            CreatedAt = DateTime.UtcNow
        }
    };

    public async Task<ServiceResponse<IEnumerable<HealthAdvice>>> GetPersonalisedHealthAdvice(string userId)
    {
        var response = new ServiceResponse<IEnumerable<HealthAdvice>>();

        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Message = "User not found";
                return response;
            }

            var random = new Random();
            var randomAdvice = AdvicePool.OrderBy(_ => random.Next()).Take(2).ToList();

            for (int i = 0; i < randomAdvice.Count; i++)
            {
                randomAdvice[i].Img = $"Health{i + 1}";
            }

            response.Data = randomAdvice;
        }
        catch (Exception ex)
        {
            response.Message = $"An error occurred: {ex.Message}";
        }

        return response;
    }
}