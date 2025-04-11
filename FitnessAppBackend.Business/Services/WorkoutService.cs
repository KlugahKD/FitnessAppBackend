using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Mapster; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class WorkoutService(ApplicationDbContext context, ILogger<WorkoutService> logger,  IWeatherService weatherService) : IWorkoutService
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

    public async Task<ServiceResponse<PagedResult<Exercise>>> GetPaginatedExercisesAsync(string userId,
        BaseFilter filter)
    {
        try
        {
            logger.LogInformation("Fetching paginated exercises for user: {UserId}", userId);

            if (!await UserExistsAsync(userId))
            {
                logger.LogDebug("User not found");
                return ResponseHelper.NotFoundResponse<PagedResult<Exercise>>("User not found");
            }

            var exercisesQuery = context.Exercises
                .Where(e => e.UserId == userId)
                .AsNoTracking();

            var totalCount = await exercisesQuery.CountAsync();

            var exercises = await exercisesQuery
                .OrderBy(e => e.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var response = new PagedResult<Exercise>
            {
                TotalCount = totalCount,
                PageSize = filter.PageSize,
                Page = filter.PageNumber,
                Payload = exercises
            };

            return ResponseHelper.OkResponse(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching paginated exercises");
            return ResponseHelper.InternalServerErrorResponse<PagedResult<Exercise>>(
                "Error fetching paginated exercises");
        }
    }

    public async Task<ServiceResponse<Exercise>> GetExerciseWithStepsAsync(string exerciseId, string userId)
    {
        try
        {
            logger.LogInformation("Fetching exercise with ID: {ExerciseId}", exerciseId);

            if (!await UserExistsAsync(userId))
            {
                logger.LogDebug("User not found");
                return ResponseHelper.NotFoundResponse<Exercise>("User not found");
            }

            var exercise = await context.Exercises
                .Include(e => e.Steps)
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (exercise == null)
            {
                logger.LogDebug("Exercise not found");
                return ResponseHelper.NotFoundResponse<Exercise>("Exercise not found");
            }

            return ResponseHelper.OkResponse(exercise);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching exercise with steps");
            return ResponseHelper.InternalServerErrorResponse<Exercise>("Error fetching exercise with steps");
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
    
     public async Task<int> GetTotalWorkoutsAsync(string userId)
    {
        logger.LogInformation("Fetching total workouts for user: {UserId}", userId);

        var totalWorkout = await context.Exercises.Where(e => e.UserId == userId).CountAsync();
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
            TotalWorkoutTime = $"{totalWorkoutTime / 60} hrs {totalWorkoutTime % 60} mins",
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
    
    public async Task<GraphDataDto> GetWorkoutGraphDataAsync(string userId)
    {
        logger.LogInformation("Fetching graph data for user: {UserId}", userId);

        // Fetch all completed workouts for the user
        var workouts = await context.Exercises
            .Where(e => e.UserId == userId && e.IsCompleted)
            .ToListAsync();

        // Group workouts by type and count them
        var groupedData = workouts
            .GroupBy(e => e.Name)
            .Select(g => new { WorkoutType = g.Key, Count = g.Count() })
            .ToList();

        // Prepare graph data
        var graphData = new GraphDataDto
        {
            X = groupedData.Select(g => g.WorkoutType).ToList(),
            Y = groupedData.Select(g => g.Count).ToList()
        };

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
            "1 time a week" => 1,
            "2-3 times a week" => 2,
            "4-5 times a week" => 4,
            "6-7 times a week" => 6,
            _ => 3
        };

        var registrationDate = user.CreatedAt.Date;
        var currentDate = DateTime.UtcNow.Date;
        var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

        var workoutDates = Enumerable.Range(0, daysInMonth)
            .Select(d => registrationDate.AddDays(d))
            .Where(date => date.Month == currentDate.Month && (date - registrationDate).Days % (7 / workoutDays) == 0)
            .Take(workoutDays * 4) 
            .ToList();

        var weatherData = await weatherService.GetCurrentWeatherAsync("Ghana") ?? new SimplifiedWeatherData()
        {
            Temperature = 25,
            IsRaining = false
        };

        foreach (var date in workoutDates)
        {
            if (weatherData is { IsRaining: true })
            {
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Indoor Cardio"));
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Yoga Session"));
            }
            else if (weatherData.Temperature > 30)
            {
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Swimming"));
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Early Morning Run"));
            }
            else if (weatherData.Temperature < 10)
            {
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Indoor HIIT"));
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Strength Training"));
            }
            else
            {
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Outdoor Running"));
                exercises.Add(CreateExercise(plan, workoutDates.IndexOf(date) + 1, date, "Cycling"));
            }
        }

        return exercises;
    }

    private Exercise CreateExercise(WorkoutPlanRequest plan, int day, DateTime workoutDate, string workoutLabel)
    {
        return plan.PlanType switch
        {
            WorkoutPlanType.LoseWeight => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Running - Day {day}",
                DurationMinutes = 30,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Date = workoutDate,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Warm up for 5 minutes",
                        DurationMinutes = 5, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Run at a steady pace",
                        DurationMinutes = 20, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Cool down for 5 minutes",
                        DurationMinutes = 5, IsCompleted = false
                    }
                }
            },
            WorkoutPlanType.GainMuscle => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Weight Lifting - Day {day} ({workoutLabel})",
                DurationMinutes = 45,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Warm up with light weights",
                        DurationMinutes = 10, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Perform 3 sets of 10 reps",
                        DurationMinutes = 30, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Cool down with stretches",
                        DurationMinutes = 5, IsCompleted = false
                    }
                }
            },
            WorkoutPlanType.ImproveEndurance => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Swimming - Day {day} ({workoutLabel})",
                DurationMinutes = 40,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Warm up with light swimming",
                        DurationMinutes = 10, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Swim at a steady pace",
                        DurationMinutes = 25, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Cool down with slow strokes",
                        DurationMinutes = 5, IsCompleted = false
                    }
                }
            },
            WorkoutPlanType.IncreaseFlexibility => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Yoga - Day {day} ({workoutLabel})",
                DurationMinutes = 60,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Start with basic poses",
                        DurationMinutes = 20, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "Hold each pose for 30 seconds",
                        DurationMinutes = 30, IsCompleted = false
                    },
                    new Step
                    {
                        Id = Guid.NewGuid().ToString("N"), Description = "End with relaxation poses",
                        DurationMinutes = 10, IsCompleted = false
                    }
                }
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<bool> UserExistsAsync(string userId)
    {
        return await context.Users.AnyAsync(u => u.Id == userId);
    }
}