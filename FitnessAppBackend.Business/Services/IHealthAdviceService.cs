using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IHealthAdviceService
{
    Task<IEnumerable<HealthAdvice>> GetPersonalizedAdviceAsync(string userId);
    Task<HealthAdvice> AddHealthAdviceAsync(HealthAdvice advice);
    Task<IEnumerable<HealthAdvice>> GetDailyTipsAsync();
}