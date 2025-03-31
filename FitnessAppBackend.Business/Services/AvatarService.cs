using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Business.Helper;
using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAppBackend.Business.Services;

public class AvatarService(ApplicationDbContext context, ILogger<AvatarService> logger) : IAvatarService
{
    public async Task<ServiceResponse<PagedResult<Avatar>>> GetAvailableAvatarsAsync(BaseFilter filter)
    {
        try
        {
            logger.LogInformation("Getting all available avatars");

            var avatars = context.Avatars.AsNoTracking().AsQueryable();
            ;
            var totalCount = await avatars.CountAsync();

            var data = await avatars
                .OrderByDescending(t => t.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(a => new Avatar
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Specialization = a.Specialization
                }).ToListAsync();

            var response = new PagedResult<Avatar>
            {
                TotalCount = totalCount,
                PageSize = filter.PageSize,
                Page = filter.PageNumber,
                Payload = data
            };

            return ResponseHelper.OkResponse(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting avatars");
            return ResponseHelper.InternalServerErrorResponse<PagedResult<Avatar>>("Error getting avatars");
        }
    }

    public async Task<ServiceResponse<Avatar>> GetAvatarByIdAsync(string id)
    {
        try
        {
            logger.LogInformation("Getting avatar by id: {Id}", id);
            var avatar = await context.Avatars.FindAsync(id);
            if (avatar != null) return ResponseHelper.OkResponse(avatar.Adapt<Avatar>());
            logger.LogDebug("Avatar with id {Id} not found", id);

            return ResponseHelper.NotFoundResponse<Avatar>("Avatar not found");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting avatar by id: {Id}", id);

            return ResponseHelper.InternalServerErrorResponse<Avatar>("Error getting avatar");
        }
    }

    public async Task<ServiceResponse<bool>> AssignAvatarToUserAsync(string userId, string avatarId)
    {
        try
        {
            logger.LogInformation("Assigning avatar {AvatarId} to user {UserId}", avatarId, userId);

            var user = await context.Users.FindAsync(userId);
            var avatar = await context.Avatars.FindAsync(avatarId);

            if (user == null || avatar == null)
            {
                logger.LogInformation("user {UserId} or avatar {AvatarId} not found", userId, avatarId);

                return ResponseHelper.NotFoundResponse<bool>("User or avatar not found");
            }

            user.PreferredAvatar = avatar.Id;
            var isSaved = await context.SaveChangesAsync() > 0;
            if (isSaved) return ResponseHelper.OkResponse(true);
            logger.LogError("Error saving changes to user {UserId}", userId);

            return ResponseHelper.InternalServerErrorResponse<bool>("Error saving changes");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error assigning avatar to user {UserId}", userId);

            return ResponseHelper.InternalServerErrorResponse<bool>("Error assigning avatar to user");
        }
    }
}