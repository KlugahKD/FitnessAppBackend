using FitnessAppBackend.Business.DTO;
using FitnessAppBackend.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController(IWeatherService weatherService) : ControllerBase
{
    [HttpGet("current/{location}")]
    public async Task<ActionResult<WeatherResponse>> GetCurrentWeather(string location)
    {
        var weatherData = await weatherService.GetCurrentWeatherAsync(location);
        return Ok(weatherData);
    }
}