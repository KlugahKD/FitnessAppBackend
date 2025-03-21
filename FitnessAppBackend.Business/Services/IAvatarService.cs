using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IAvatarService
{
    Task<IEnumerable<Avatar>> GetAvailableAvatarsAsync();
    Task<Avatar> GetAvatarByIdAsync(int id);
    Task<Avatar> AssignAvatarToUserAsync(string userId, int avatarId);
}