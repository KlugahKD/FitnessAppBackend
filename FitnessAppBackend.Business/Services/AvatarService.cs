using FitnessAppBackend.Data.Data;
using FitnessAppBackend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppBackend.Business.Services;

public class AvatarService(ApplicationDbContext context) : IAvatarService
{
    public async Task<IEnumerable<Avatar>> GetAvailableAvatarsAsync()
    {
        return await context.Avatars.ToListAsync();
    }

    public async Task<Avatar> GetAvatarByIdAsync(int id)
    {
        var avatar = await context.Avatars.FindAsync(id);
        return avatar ?? throw new KeyNotFoundException("Avatar not found");
    }

    public async Task<Avatar> AssignAvatarToUserAsync(string userId, int avatarId)
    {
        var user = await context.Users.FindAsync(userId);
        var avatar = await context.Avatars.FindAsync(avatarId);

        if (user == null || avatar == null)
        {
            throw new KeyNotFoundException("User or Avatar not found");
        }

        user.PreferredAvatar = avatar.Id.ToString();
        await context.SaveChangesAsync();

        return avatar;
    }
}