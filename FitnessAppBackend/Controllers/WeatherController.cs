using FitnessAppBackend.Business.Services;
using FitnessAppBackend.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WeatherController(IWeatherService weatherService) : ControllerBase
{
    [HttpGet("current/{location}")]
    public async Task<ActionResult<WeatherData>> GetCurrentWeather(string location)
    {
        var weatherData = await weatherService.GetCurrentWeatherAsync(location);
        return Ok(weatherData);
    }

    [HttpGet("suggestions/{location}")]
    public async Task<ActionResult<IEnumerable<string>>> GetWorkoutSuggestions(string location)
    {
        var weatherData = await weatherService.GetCurrentWeatherAsync(location);
        var suggestions = await weatherService.GetWorkoutSuggestionsAsync(weatherData);
        return Ok(suggestions);
    }
}