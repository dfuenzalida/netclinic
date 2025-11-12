using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Services;

namespace NetClinic.Api;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherSummaryService _weatherSummaryService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IWeatherSummaryService weatherSummaryService)
    {
        _logger = logger;
        _weatherSummaryService = weatherSummaryService;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("WeatherForecast GET request received at {Timestamp}", DateTime.UtcNow);
        
        try
        {
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                _weatherSummaryService.GetRandomSummary()
            ))
            .ToArray();

            _logger.LogInformation("Successfully generated {Count} weather forecasts", forecasts.Length);
            return forecasts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating weather forecasts");
            throw;
        }
    }
}