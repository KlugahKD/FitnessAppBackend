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

            var planExists =
                await context.WorkoutPlans.AnyAsync(
                    p => p.UserId == plan.UserId && p.Description == plan.PlanType.ToString());
            if (planExists)
            {
                logger.LogDebug("Workout plan already exists");

                return ResponseHelper.BadRequestResponse<WorkoutPlan>("Workout plan already exists");
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

        var weatherData = await weatherService.GetCurrentWeatherAsync("Ghana");

        for (int i = 0; i < workoutDays; i++)
        {
            if (weatherData.IsRaining)
            {
                // Indoor exercises for rainy weather
                exercises.Add(CreateExercise(plan, i + 1, "Indoor Cardio"));
                exercises.Add(CreateExercise(plan, i + 1, "Yoga Session"));
            }
            else if (weatherData.Temperature > 30)
            {
                // Hot weather exercises
                exercises.Add(CreateExercise(plan, i + 1, "Swimming"));
                exercises.Add(CreateExercise(plan, i + 1, "Early Morning Run"));
            }
            else if (weatherData.Temperature < 10)
            {
                // Cold weather exercises
                exercises.Add(CreateExercise(plan, i + 1, "Indoor HIIT"));
                exercises.Add(CreateExercise(plan, i + 1, "Strength Training"));
            }
            else
            {
                // Moderate weather exercises
                exercises.Add(CreateExercise(plan, i + 1, "Outdoor Running"));
                exercises.Add(CreateExercise(plan, i + 1, "Cycling"));
            }
        }

        return exercises;
    }
    private Exercise CreateExercise(WorkoutPlanRequest plan, int day, string workoutLabel)
    {
        return plan.PlanType switch
        {
            WorkoutPlanType.LoseWeight => new Exercise
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Running - Day {day} ({workoutLabel})",
                DurationMinutes = 30,
                IsStarted = false,
                IsCompleted = false,
                UserId = plan.UserId,
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