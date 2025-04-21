using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class DataSeeder
{
    public static async Task SeedAvatars(IServiceProvider serviceProvider)
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
                        "Every drop of sweat is a badge of honor. Keep moving!",
                        "When your legs scream stop, your heart says go. Listen to your heart.",
                        "You’re not just running — you’re rewriting limits.",
                        "Breathe through the burn. That’s your breakthrough moment.",
                        "No matter the speed, every step is a victory.",
                        "Push past your comfort zone — that’s where the magic happens.",
                        "The road is long, but your endurance is longer.",
                        "Pain is temporary. The pride of finishing lasts forever.",
                        "Your lungs are expanding, your heart is growing stronger. This is transformation.",
                        "You’ve already conquered the hardest part — starting. Now finish strong.",
                        "You’re a machine — a relentless, heart-pounding, goal-chasing machine!"
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
                        "Each rep carves greatness. Lift like you mean it.",
                        "Strength isn't built in comfort — it's earned through grit.",
                        "Today’s pain is tomorrow’s power. Own every set.",
                        "Iron sharpens iron. Be unbreakable.",
                        "Muscles grow when excuses shrink.",
                        "Pick up that bar like it owes you something.",
                        "Dominate the weights. Let the world hear your roar.",
                        "The grind never lies — every lift is progress.",
                        "Train like a warrior, fuel like a beast, rest like a king.",
                        "No lift is wasted when the goal is greatness.",
                        "You’re not just lifting weights — you’re building a legacy."
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
                        "You’re not slowing down — you’re syncing with your strength.",
                        "Recovery is a form of discipline. Honor the stillness.",
                        "You can’t pour from an empty cup. Refill with intention.",
                        "A strong body needs a calm mind. Breathe through the chaos.",
                        "Stretch, restore, and return stronger than ever.",
                        "You grow in rest as much as you do in reps.",
                        "Inhale peace. Exhale tension. You are your sanctuary.",
                        "Healing is not quitting. It’s preparation for a bigger comeback.",
                        "Progress isn’t always loud. Sometimes it’s quiet and powerful.",
                        "Rest, recharge, rise. Your wellness is your weapon."
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
                        "A strong core is your foundation — build it brick by brick.",
                        "Control is power. Feel every contraction count.",
                        "Abs aren’t made overnight. They’re built in every rep and every meal.",
                        "Balance. Strength. Precision. Your core drives it all.",
                        "Don’t just go through the motion — own it with control.",
                        "Your posture, your movement, your strength — all from your center.",
                        "No shortcuts to a six-pack — just consistency and grit.",
                        "Hold the plank. Embrace the shake. That’s strength being sculpted.",
                        "Each crunch, each twist — you’re unlocking inner power.",
                        "Let your core speak louder than your excuses."
                    }
                    ,
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