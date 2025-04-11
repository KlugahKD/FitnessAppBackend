using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class AvatarService(ApplicationDbContext context, ILogger<AvatarService> logger) : IAvatarService
{
    public async Task<ServiceResponse<string>> GetResponseAsync(string userId, string question)
    {
        try
        {
            logger.LogInformation("User with Id {Id}, is Interacting with avatar", userId);
            var user = await context.Users.Include(u => u.PreferredAvatar).FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.PreferredAvatar == null)
            {
                logger.LogDebug("User does not have an avatar");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            var avatar = await context.Avatars.FirstOrDefaultAsync(a => a.Name == user.PreferredAvatar);
            if (avatar == null)
            {
                logger.LogDebug("Avatar not found");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            if (avatar.Responses.TryGetValue(question, out var response))
            {
                return ResponseHelper.OkResponse(response);
            }

            return ResponseHelper.OkResponse("I don't have an answer for that.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting response for user with Id {Id}", userId);
            return ResponseHelper.InternalServerErrorResponse<string>(
                "An error occurred while processing your request.");
        }
    }

    public async Task<ServiceResponse<string>> GetMotivationalMessageAsync(string userId)
    {
        try
        {
            logger.LogInformation("Fetching motivational message for user with Id {Id}", userId);
            var user = await context.Users.Include(u => u.PreferredAvatar).FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.PreferredAvatar == null)
            {
                logger.LogDebug("User does not have an avatar");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            var avatar = await context.Avatars.FirstOrDefaultAsync(a => a.Name == user.PreferredAvatar);
            if (avatar == null)
            {
                logger.LogDebug("Avatar not found");
                return ResponseHelper.NotFoundResponse<string>("Avatar not found.");
            }

            var random = new Random();
            var message = avatar.MotivationalMessages[random.Next(avatar.MotivationalMessages.Count)];
            return ResponseHelper.OkResponse(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting motivational message for user with Id {Id}", userId);
            return ResponseHelper.InternalServerErrorResponse<string>(
                "An error occurred while processing your request.");
        }
    }
}