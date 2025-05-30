using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class WorkoutService(ApplicationDbContext context, ILogger<WorkoutService> logger,  IWeatherService weatherService,  UserManager<ApplicationUser> userManager) : IWorkoutService
{
    public async Task<ServiceResponse<WorkoutPlan>> CreateWorkoutPlanAsync(WorkoutPlanRequest plan)
    {
        try
        {
            logger.LogInformation("Creating workout plan");

            if (!await UserExistsAsync(plan.UserId))
            {
                logger.LogDebug("User not found");
                return ResponseHelper.NotFoundResponse<WorkoutPlan>("User not found");
            }

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var existingPlan = await context.WorkoutPlans
                .Where(p => p.UserId == plan.UserId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingPlan != null && 
                existingPlan.CreatedAt.Month == currentMonth && 
                existingPlan.CreatedAt.Year == currentYear)
            {
                logger.LogDebug("Workout plan already exists for the current month");
                return ResponseHelper.BadRequestResponse<WorkoutPlan>("Workout plan already exists for the current month");
            }

            var exercises = await GenerateExercisesAsync(plan);

            var workoutPlan = new WorkoutPlan()
            {
                Id = Guid.NewGuid().ToString("N"),
                UserId = plan.UserId,
                Description = plan.PlanType.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Exercises = exercises
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

    public async Task<ServiceResponse<List<Exercise>>> GetExercisesForTodayAsync(string userId)
    {
        try
        {
            logger.LogInformation("Fetching exercises for today for user: {UserId}", userId);

            var today = DateTime.UtcNow.Date;

            var exercises = await context.Exercises
                .Where(e => e.UserId == userId && e.Date.Date == today && !e.IsCompleted)
                .OrderBy(e => e.Name)
                .AsNoTracking()
                .ToListAsync();
            
            for (int i = 0; i < exercises.Count; i++)
            {
                exercises[i].Img = $"workout{i + 1}";
            }

            return ResponseHelper.OkResponse(exercises);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching exercises for today");
            return ResponseHelper.InternalServerErrorResponse<List<Exercise>>("Error fetching exercises for today");
        }
    }

    public async Task<ServiceResponse<Exercise>> GetExerciseWithStepsAsync(string exerciseId, string userId)
    {
        try
        {
            logger.LogInformation("Fetching exercise with ID: {ExerciseId}", exerciseId);

            var exercise = await context.Exercises
                .Include(e => e.Steps)
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (exercise == null)
            {
                logger.LogDebug("Exercise not found");
                return ResponseHelper.NotFoundResponse<Exercise>("Exercise not found");
            } 
            
            exercise.Img = $"workout{new Random().Next(1, 5)}";
            
            return ResponseHelper.OkResponse(exercise);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching exercise with steps");
            return ResponseHelper.InternalServerErrorResponse<Exercise>("Error fetching exercise with steps");
        }
    }
    
    public async Task<ServiceResponse<WorkoutSummaryDto>> GetCompletedAndMissedWorkoutsAsync(string userId)
    {
        try
        {
            
            var pastWorkouts = await context.Exercises
                .Where(e => e.UserId == userId && (e.IsCompleted || e.IsMissed))
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToListAsync();
            
                for (int i = 0; i < pastWorkouts.Count; i++)
                {
                    pastWorkouts[i].Img = $"workout{i + 1}";
                }
    
            var result = new WorkoutSummaryDto
            {
                PastWorkouts = pastWorkouts
            };
    
            return ResponseHelper.OkResponse(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching completed and missed workouts");
            return ResponseHelper.InternalServerErrorResponse<WorkoutSummaryDto>("Error fetching completed and missed workouts");
        }
    }

    public async Task<ServiceResponse<WorkoutPlan>> UpdateWorkoutPlanAsync(string planId,
        WorkoutPlanRequest updateRequest)
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

            var exercises = await GenerateExercisesAsync(updateRequest); 


            plan.UpdatedAt = DateTime.UtcNow;
            plan.Exercises = exercises;
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
    
    public async Task<ServiceResponse<bool>> MarkExerciseAsCompletedAsync(string exerciseId, string userId)
    {
        try
        {
            logger.LogInformation("Marking exercise with ID: {ExerciseId} as completed", exerciseId);
        
            var exercise = await context.Exercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (exercise == null || exercise.IsCompleted || exercise.IsMissed)
            {
                logger.LogDebug("Exercise not found or already completed");
                return ResponseHelper.NotFoundResponse<bool>("Cant mark this exercise as complete");
            }

            var steps = await context.Steps
                .Where(s => s.ExerciseId == exerciseId)
                .ToListAsync();

            if (steps.Any(s => !s.IsCompleted))
            {
                logger.LogDebug("Not all steps are completed for exercise ID: {ExerciseId}", exerciseId);
                return ResponseHelper.BadRequestResponse<bool>("Not all steps are completed.");
            }

            exercise.IsCompleted = true;

            await context.SaveChangesAsync();

            return ResponseHelper.OkResponse(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error marking exercise as completed");
            return ResponseHelper.InternalServerErrorResponse<bool>("Error marking exercise as completed.");
        }
    }
    
    public async Task<ServiceResponse<bool>> MarkStepAsCompletedAsync(string stepId, string userId)
    {
        try
        {
            logger.LogInformation("Marking step with ID: {StepId} as completed", stepId);

            var step = await context.Steps.FirstOrDefaultAsync(s => s.Id == stepId);
            if (step == null)
            {
                logger.LogDebug("Step not found");
                return ResponseHelper.NotFoundResponse<bool>("Step not found.");
            }

            var exercise = await context.Exercises.FirstOrDefaultAsync(e => e.Id == step.ExerciseId && e.UserId == userId);
            if (exercise == null || exercise.IsCompleted || exercise.IsMissed)
            {
                logger.LogDebug("Exercise not found or does not belong to the user");
                return ResponseHelper.NotFoundResponse<bool>("Cant mark this step as complete");
            }

            if (step.IsCompleted)
            {
                logger.LogDebug("Step is already completed");
                return ResponseHelper.BadRequestResponse<bool>("Step is already completed.");
            }

            step.IsCompleted = true;
            exercise.IsStarted = true;

            await context.SaveChangesAsync(); 

            var allStepsCompleted = await context.Steps
                .Where(s => s.ExerciseId == exercise.Id)
                .AllAsync(s => s.IsCompleted);

            if (allStepsCompleted)
            {
                exercise.IsCompleted = true;
                await context.SaveChangesAsync(); 
            }

            return ResponseHelper.OkResponse(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error marking step as completed");
            return ResponseHelper.InternalServerErrorResponse<bool>("Error marking step as completed.");
        }
    }
    
     public async Task<int> GetTotalWorkoutsAsync(string userId)
    {
        logger.LogInformation("Fetching total workouts for user: {UserId}", userId);

        var totalWorkout = await context.Exercises.Where(e => e.UserId == userId && e.IsCompleted).CountAsync();
        if (totalWorkout == 0)
        {
            logger.LogWarning("No workouts found for user: {UserId}", userId);
            return 0;
        }

        return totalWorkout;
    }

    public async Task<WeeklyStatsDto> GetWeeklyWorkoutStatsAsync(string userId)
    {
        logger.LogInformation("Fetching weekly workout stats for user: {UserId}", userId);

        var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);

        var workouts = await context.Exercises
            .Where(e => e.UserId == userId && e.Date >= startOfWeek && e.Date < endOfWeek)
            .ToListAsync();

        var completedWorkouts = workouts.Count(e => e.IsCompleted);
        var totalWorkoutTime = workouts.Where(e => e.IsCompleted).Sum(e => e.DurationMinutes);

        return new WeeklyStatsDto
        {
            CompletedWorkouts = completedWorkouts,
            TotalWorkoutsForTheWeek = workouts.Count,
            TotalWorkoutTime = totalWorkoutTime,
            DaysWorkedOut = workouts.Where(e => e.IsCompleted).Select(e => e.Date.Date).Distinct().Count()
        };
    }

    public async Task<int> CalculateWorkoutStreakAsync(string userId)
    {
        logger.LogInformation("Calculating workout streak for user: {UserId}", userId);

        var exercises = await context.Exercises
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        int streak = 0;
        DateTime? lastWorkoutDate = null;

        foreach (var exercise in exercises)
        {
            if (!exercise.IsCompleted) break;

            if (lastWorkoutDate == null || exercise.Date.Date == lastWorkoutDate.Value.AddDays(-1).Date)
            {
                streak++;
                lastWorkoutDate = exercise.Date.Date;
            }
            else break;
        }

        return streak;
    }
    
    public async Task<List<GraphDataItem>> GetWorkoutGraphDataAsync(string userId)
    {
        var workouts = await context.Exercises
            .Where(e => e.UserId == userId && e.IsCompleted)
            .ToListAsync();

        var groupedData = workouts
            .GroupBy(e => e.Date.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        var startDate = DateTime.Today.AddDays(-6);
        var graphData = Enumerable.Range(0, 7)
            .Select(i =>
            {
                var date = startDate.AddDays(i);
                return new GraphDataItem
                {
                    X = $"Day {i + 1}",
                    Y = groupedData.GetValueOrDefault(date, 0)
                };
            })
            .ToList();

        return graphData;
    }

   private async Task<List<Exercise>> GenerateExercisesAsync(WorkoutPlanRequest plan)
{
    var user = await context.Users.FindAsync(plan.UserId);
    if (user == null)
    {
        throw new ArgumentException("User not found");
    }

    var exercises = new List<Exercise>();
    int workoutDays = user.HowOftenWorkOut switch
    {
        "1-2 times a week" => 2,
        "3-4 times a week" => 4,
        "5-6 times a week" => 5,
        "Everyday" => 7,
        _ => 3
    };

    var registrationDate = user.CreatedAt.Date;
    var currentDate = DateTime.UtcNow.Date;
    var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

    var workoutDates = Enumerable.Range(0, daysInMonth)
        .Select(d => registrationDate.AddDays(d))
        .Where(date => date.Month == currentDate.Month && workoutDays > 0 &&
                       (date - registrationDate).Days % Math.Max(1, (7 / workoutDays)) == 0)
        .Take(workoutDays * 4)
        .ToList();

    var weatherData = await weatherService.GetCurrentWeatherAsync("Ghana") ?? new SimplifiedWeatherData
    {
        Temperature = 25,
        IsRaining = false
    };

    if (workoutDates.Count == 0)
    {
        logger.LogWarning("No workout dates generated. RegistrationDate: {RegistrationDate}, CurrentDate: {CurrentDate}, WorkoutDays: {WorkoutDays}",
            registrationDate, currentDate, workoutDays);
    }

    foreach (var date in workoutDates)
    {
        int index = workoutDates.IndexOf(date) + 1;

        if (weatherData.IsRaining)
        {
            exercises.Add(CreateExercise(plan, index, date, "Indoor Power Burn"));
            exercises.Add(CreateExercise(plan, index, date, "Rainy Day Flow & Flex"));
        }
        else if (weatherData.Temperature > 30)
        {
            exercises.Add(CreateExercise(plan, index, date, "Heatwave Pool Shred"));
            exercises.Add(CreateExercise(plan, index, date, "Sunrise Sprint Challenge"));
        }
        else if (weatherData.Temperature < 10)
        {
            exercises.Add(CreateExercise(plan, index, date, "Warm-Up HIIT Express"));
            exercises.Add(CreateExercise(plan, index, date, "Cozy Core Strength"));
        }
        else
        {
            exercises.Add(CreateExercise(plan, index, date, "Outdoor Endurance Run"));
            exercises.Add(CreateExercise(plan, index, date, "Power Pedal Circuit"));
        }
    }

    return exercises;
}

    
    public async Task<WeeklyStatsDto> GetLastWeekWorkoutStatsAsync(string userId)
    {
        var oneWeekAgo = DateTime.Today.AddDays(-7);
        var today = DateTime.Today;

        var lastWeekWorkouts = await context.Exercises
            .Where(e => e.UserId == userId && e.IsCompleted && e.Date.Date >= oneWeekAgo && e.Date.Date < today)
            .ToListAsync();

        var totalWorkoutTime = lastWeekWorkouts.Sum(e => e.DurationMinutes);

        var daysWorkedOut = lastWeekWorkouts
            .Select(e => e.Date.Date)
            .Distinct()
            .Count();

        return new WeeklyStatsDto
        {
            CompletedWorkouts = lastWeekWorkouts.Count,
            TotalWorkoutTime = totalWorkoutTime,
            TotalWorkoutsForTheWeek = 7, 
            DaysWorkedOut = daysWorkedOut
        };
    }

    private static Exercise CreateExercise(WorkoutPlanRequest plan, int day, DateTime workoutDate, string workoutLabel)
    {
        return plan.PlanType switch
        {
            WorkoutPlanType.LoseWeight => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"HIIT Burn - Day {day} ({workoutLabel})",
                Description = "High-Intensity Interval Training designed to burn calories and boost metabolism.",
                DurationMinutes = 35,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Date = workoutDate,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Dynamic warm-up: Jumping jacks + high knees",
                        DurationMinutes = 5, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "4 Rounds: Burpees, Mountain Climbers, Jump Squats (30s each, 15s rest)",
                        DurationMinutes = 20, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Core finisher: Plank variations + Bicycle Crunches", DurationMinutes = 5,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Cool down & stretch", DurationMinutes = 5,
                        IsCompleted = false
                    }
                }
            },

            WorkoutPlanType.GainMuscle => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Strength Training - Day {day} ({workoutLabel})",
                Description = "Targeted hypertrophy workout to build lean muscle mass.",
                DurationMinutes = 50,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Date = workoutDate,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Warm-up: Resistance band drills + foam rolling", DurationMinutes = 10,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Barbell Compound Lifts (Bench/Deadlift/Squat)", DurationMinutes = 20,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Isolation sets: Bicep curls, triceps dips, leg extensions", DurationMinutes = 15,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Post-lift stretch & cooldown",
                        DurationMinutes = 5, IsCompleted = false
                    }
                }
            },

            WorkoutPlanType.ImproveEndurance => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Endurance Bootcamp - Day {day} ({workoutLabel})",
                Description = "Stamina-building routine combining cardio and resistance elements.",
                DurationMinutes = 45,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Date = workoutDate,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Warm-up jog + jump rope", DurationMinutes = 5,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Cycling or Rowing Machine: 20 minutes steady pace", DurationMinutes = 20,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Treadmill Sprints: 5 rounds of 1 min sprint / 1 min walk", DurationMinutes = 15,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Stretch and deep breathing",
                        DurationMinutes = 5, IsCompleted = false
                    }
                }
            },

            WorkoutPlanType.IncreaseFlexibility => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Flex & Flow - Day {day} ({workoutLabel})",
                Description = "A guided flexibility and mobility session designed to unlock full-body range of motion.",
                DurationMinutes = 50,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Date = workoutDate,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Foam rolling & joint mobility drills",
                        DurationMinutes = 10, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Dynamic stretches: World’s greatest stretch, leg swings", DurationMinutes = 15,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Description = "Yoga Flow: Sun Salutation + Pigeon pose + Bridge pose", DurationMinutes = 20,
                        IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Guided relaxation & breathwork",
                        DurationMinutes = 5, IsCompleted = false
                    }
                }
            },

            _ => throw new ArgumentOutOfRangeException()
        };
    }


    private async Task<bool> UserExistsAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        return user != null;
    }
}