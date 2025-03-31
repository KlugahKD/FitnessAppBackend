using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IAvatarService
{
    Task<ServiceResponse<PagedResult<Avatar>>> GetAvailableAvatarsAsync(BaseFilter filter);
    Task<ServiceResponse<Avatar>> GetAvatarByIdAsync(string id);  
    Task<ServiceResponse<bool>> AssignAvatarToUserAsync(string userId, string avatarId);
}