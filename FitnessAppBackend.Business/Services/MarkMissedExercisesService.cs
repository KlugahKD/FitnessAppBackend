using FitnessAppBackend.Data.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class MarkMissedExercisesService(IServiceProvider serviceProvider, ILogger<MarkMissedExercisesService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MarkMissedExercisesService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var today = DateTime.UtcNow.Date;

                var missedExercises = context.Exercises
                    .Where(e => !e.IsCompleted && e.Date.Date < today)
                    .ToList();

                foreach (var exercise in missedExercises)
                {
                    exercise.IsMissed = true;
                }

                await context.SaveChangesAsync(stoppingToken);

                logger.LogInformation("Marked {Count} exercises as missed.", missedExercises.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while marking missed exercises.");
            }

            // Run the task daily
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        logger.LogInformation("MarkMissedExercisesService is stopping.");
    }
}