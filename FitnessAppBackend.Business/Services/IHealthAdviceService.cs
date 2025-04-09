using FitnessAppBackend.Business.Common;
using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IHealthAdviceService
{
    Task<ServiceResponse<IEnumerable<HealthAdvice>>> GetPersonalisedHealthAdvice(string userId);
}