using FitnessAppBackend.Business.Common;

namespace FitnessAppBackend.Business.Services;

public interface IAvatarService
{
    Task<ServiceResponse<string>> GetResponseAsync(string userId, string question);
    Task<ServiceResponse<string>> GetMotivationalMessageAsync(string userId);
}