using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class DataSeeder
{
    public async Task SeedAvatars(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

        if (context.Avatars.Any())
        {
            logger.LogInformation("Avatars already seeded.");
            return;
        }
        if (!context.Avatars.Any())
        {
            var avatars = new List<Avatar>
            {
                new Avatar
                {
                    Name = "Pulse",
                    Description = "Driven by heart-pumping cardio and endurance goals.",
                    Specialization = "Cardio",
                    MotivationalMessages = new List<string>
                    {
                        "One more mile, one step closer.",
                        "Breathe deep, run free.",
                        "Endurance is built, not born.",
                        "The finish line is just a mindset.",
                        "Your legs will forgive you. Your goals won’t.",
                        "Push past the burn — that’s growth.",
                        "You’re not tired, you’re transforming.",
                        "Feel your heart beat for greatness.",
                        "Fast or slow, just don’t stop.",
                        "You’re lapping everyone on the couch."
                    },
                    Responses = new Dictionary<string, string>
                    {
                        { "How can I run longer?", "Gradually increase distance and pace weekly." },
                        { "Is daily running okay?", "Yes, but include active recovery days to avoid injury." },
                        { "What improves heart health?", "Consistent cardio like brisk walking, cycling, or running." },
                        { "Can I lose fat by walking?", "Absolutely, especially with a proper diet." },
                        { "How do I pace myself?", "Use a talk test — if you can’t speak, slow down." },
                        { "Should I eat before cardio?", "Light carbs 30–60 minutes before can help." },
                        { "What's better: HIIT or LISS?", "Both are effective — alternate for variety and results." },
                        { "What’s the runner’s high?", "A real boost of endorphins after sustained effort!" }
                    }
                },
                new Avatar
                {
                    Name = "Titan",
                    Description = "Muscle-building powerhouse focused on lifting and strength.",
                    Specialization = "Strength Training",
                    MotivationalMessages = new List<string>
                    {
                        "Pick it up. Put it down. Repeat until beast mode.",
                        "Strong isn’t a look — it’s a mindset.",
                        "The barbell is your best friend.",
                        "Train smart. Lift heavy. Eat well.",
                        "Every rep is a step closer to power.",
                        "Grow through what you go through.",
                        "Pain is temporary. Pride is forever.",
                        "Big lifts. Bigger mindset.",
                        "Muscles are earned, not bought.",
                        "Your future self is flexing already."
                    },
                    Responses = new Dictionary<string, string>
                    {
                        { "How can I build strength?", "Use compound lifts and progressive overload." },
                        { "How many reps for muscle gain?", "8–12 reps per set is ideal for hypertrophy." },
                        { "What should I eat for muscle?", "High-protein meals and a slight calorie surplus." },
                        {
                            "Can I train two muscle groups a day?",
                            "Yes, pair them smartly like push/pull or back/biceps."
                        },
                        { "How do I avoid injury?", "Warm up, stretch, and focus on proper form always." },
                        { "Should I take creatine?", "It’s one of the most researched and effective supplements." },
                        { "Do I need a spotter?", "Yes, especially for heavy bench presses or squats." },
                        {
                            "What’s better: dumbbells or barbells?",
                            "Both have benefits. Use them to complement each other."
                        }
                    }
                },
                new Avatar
                {
                    Name = "Zenith",
                    Description = "Focused on inner strength, recovery, and wellness.",
                    Specialization = "Wellness & Recovery",
                    MotivationalMessages = new List<string>
                    {
                        "Balance is the new hustle.",
                        "Rest isn’t weakness — it’s strategy.",
                        "A calm mind is a strong foundation.",
                        "Stretch. Breathe. Thrive.",
                        "Wellness is your competitive edge.",
                        "Recharge. You’ve earned it.",
                        "You’re healing — not stalling.",
                        "Hydrate your hustle.",
                        "Good sleep fuels good performance.",
                        "Take care of your body — it’s the only one you get."
                    },
                    Responses = new Dictionary<string, string>
                    {
                        { "Why is sleep important for fitness?", "It’s when your body repairs and builds muscle." },
                        { "How can I recover faster?", "Sleep more, hydrate, and eat anti-inflammatory foods." },
                        { "What’s foam rolling for?", "It relieves muscle tightness and improves mobility." },
                        { "Should I take rest days?", "Yes! They prevent burnout and aid long-term progress." },
                        { "How to reduce stress?", "Breathing techniques, meditation, and stretching help." },
                        { "What are good recovery foods?", "Think: berries, bananas, eggs, salmon, and leafy greens." },
                        { "Can yoga help fitness?", "Absolutely — improves flexibility, focus, and breathing." },
                        { "What’s the importance of hydration?", "It affects energy, strength, and metabolism." }
                    }
                },
                new Avatar
                {
                    Name = "Core",
                    Description = "All about strength from the inside out — abs, posture, and power.",
                    Specialization = "Core & Stability",
                    MotivationalMessages = new List<string>
                    {
                        "Strong core, strong body.",
                        "Abs are built with reps and kitchen discipline.",
                        "Balance starts from the center.",
                        "Feel the burn — it's working.",
                        "No shortcuts to a strong core.",
                        "Stability is strength you can’t always see.",
                        "Tighten up, level up.",
                        "Your powerhouse is your core.",
                        "Let’s sculpt that strength!",
                        "Control the movement. Don’t let it control you."
                    },
                    Responses = new Dictionary<string, string>
                    {
                        { "How can I get visible abs?", "Reduce body fat through diet, cardio, and ab workouts." },
                        { "Are crunches enough?", "Mix crunches with planks, leg raises, and twists." },
                        { "How often should I train core?", "2–4 times a week is ideal." },
                        { "Can I train abs daily?", "Yes, but vary intensity and give rest if sore." },
                        { "What’s the best ab move?", "Planks build both strength and endurance." },
                        { "Does diet matter?", "More than you think. Abs are made in the kitchen." },
                        { "Can I use weights for abs?", "Yes! Weighted core exercises build definition." },
                        { "What are obliques?", "Side ab muscles — target them with side planks, twists, etc." }
                    }
                }
            };

            context.Avatars.AddRange(avatars);
            await context.SaveChangesAsync();
            logger.LogInformation("Avatars seeded.");
        }
    }
}