using FitnessAppBackend.Data.Models;

namespace FitnessAppBackend.Business.Services;

public interface IWeatherService
{
    Task<WeatherData> GetCurrentWeatherAsync(string location);
    Task<IEnumerable<string>> GetWorkoutSuggestionsAsync(WeatherData weatherData);
}