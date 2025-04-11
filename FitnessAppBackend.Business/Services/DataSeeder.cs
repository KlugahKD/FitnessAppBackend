using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public static class DataSeeder
{
    public static async Task SeedAvatars(ApplicationDbContext context)
    {
        if (!context.Avatars.Any())
        {
            var avatars = new List<Avatar>
            {
                new Avatar
                {
                    Name = "Fitness Guru",
                    Description = "Expert in fitness and health.",
                    Specialization = "Fitness",
                    MotivationalMessages = new List<string>
                    {
                        "Keep pushing your limits!",
                        "Every step counts towards your goal."
                    },
                    Responses = new Dictionary<string, string>
                    {
                        { "How can I lose weight?", "Focus on a calorie deficit and regular exercise." },
                        { "How do I build muscle?", "Incorporate strength training and eat enough protein." }
                    }
                },
                // Add 3 more avatars with unique characteristics
            };

            context.Avatars.AddRange(avatars);
            await context.SaveChangesAsync();
        }
    }
}