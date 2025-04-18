using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Helper;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

/// <summary>
/// Service for handling dashboard-related operations.
/// </summary>
public class DashboardService(
    IWeatherService weatherService,
    IWorkoutService workoutService,
    IAvatarService avatarService,
    ILogger<DashboardService> logger)
    : IDashboardService
{
    public async Task<ServiceResponse<DashboardOverview>> GetDashboardOverviewAsync(string userId)
    {
        try
        {
            logger.LogInformation("Fetching dashboard overview for user: {UserId}", userId);

            // Fetch weather information
            var weather = await weatherService.GetCurrentWeatherAsync("Accra, Ghana");
            if (weather == null)
            {
                logger.LogWarning("Weather data not found for user: {UserId}", userId);
                weather = new SimplifiedWeatherData
                {
                    Temperature = 29.7,
                    IsRaining = false
                };
            }

            // Fetch total workouts
            var totalWorkouts = await workoutService.GetTotalWorkoutsAsync(userId);

            // Fetch weekly workout stats
            var weeklyStats = await workoutService.GetWeeklyWorkoutStatsAsync(userId);

            // Calculate workout streak
            var streak = await workoutService.CalculateWorkoutStreakAsync(userId);

            // Fetch motivational messages
            var motivationalMessages = await avatarService.GetMotivationalMessageAsync(userId);

            // Fetch graph data
            var graphData = await workoutService.GetWorkoutGraphDataAsync(userId);

            var avgWorkoutTime = (weeklyStats.TotalWorkoutTime) /
                                 (weeklyStats.DaysWorkedOut > 0 ? weeklyStats.DaysWorkedOut : 1);
            var goalCompletionPercentage = (weeklyStats.DaysWorkedOut / 7.0) * 100;

            var data = new DashboardOverview
            {
                Temperature = weather.Temperature,
                TotalWorkouts = totalWorkouts,
                Streak = streak,
                WeeklyStats = new WeeklyStatsDto
                {
                    CompletedWorkouts = weeklyStats.CompletedWorkouts,
                    TotalWorkoutTime = weeklyStats.TotalWorkoutTime,
                    TotalWorkoutsForTheWeek = weeklyStats.TotalWorkoutsForTheWeek,
                    DaysWorkedOut = weeklyStats.DaysWorkedOut
                },
                DaysWorkedOut = weeklyStats.DaysWorkedOut,
                MotivationalMessage = motivationalMessages.Data,
                GraphData = new GraphDataDto
                {
                    Y = graphData.Select((g, index) => new GraphPoint
                    {
                        Name = Enum.GetName(typeof(DayOfWeek), (index % 7)) ?? "Unknown",
                        Total = g.Y
                    }).ToList()
                },
                AvgWorkoutTime = $"{avgWorkoutTime} min",
                GoalCompletionPercentage = goalCompletionPercentage,
                GoalCompletionDetails = $"{weeklyStats.DaysWorkedOut} of 6 days"
            };

            return ResponseHelper.OkResponse(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching dashboard overview for user: {UserId}", userId);
            return ResponseHelper.InternalServerErrorResponse<DashboardOverview>(
                "An error occurred while fetching the dashboard overview.");
        }
    }
}