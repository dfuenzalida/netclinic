namespace NetClinic.Api.Services;

public class WeatherSummaryService : IWeatherSummaryService
{
    private readonly List<string> _summaries;
    private readonly ILogger<WeatherSummaryService> _logger;

    public WeatherSummaryService(ILogger<WeatherSummaryService> logger)
    {
        _logger = logger;
        _summaries = new List<string>
        {
            "Freezing",
            "Bracing", 
            "Chilly", 
            "Cool", 
            "Mild", 
            "Warm", 
            "Balmy", 
            "Hot", 
            "Sweltering", 
            "Scorching"
        };

        _logger.LogInformation("WeatherSummaryService initialized with {Count} summaries", _summaries.Count);
    }

    public string GetRandomSummary()
    {
        var randomSummary = _summaries[Random.Shared.Next(_summaries.Count)];
        _logger.LogDebug("Selected random weather summary: {Summary}", randomSummary);
        return randomSummary;
    }
}