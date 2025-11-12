using NetClinic.Api;
using NetClinic.Api.Services;

namespace NetClinic.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        
        // Register custom services
        builder.Services.AddScoped<IWeatherSummaryService, WeatherSummaryService>();
        builder.Services.AddScoped<IVetService, VetService>();

        var app = builder.Build();

        // Add request logging middleware
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Won't use HTTPS directly
        // app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
