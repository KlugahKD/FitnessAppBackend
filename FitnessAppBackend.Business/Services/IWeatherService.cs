using FitnessAppBackend.Business.DTO;

namespace FitnessAppBackend.Business.Services;

public interface IWeatherService
{
    Task<SimplifiedWeatherData?> GetCurrentWeatherAsync(string city);
}