using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Services;
using NetClinic.Api.Utils;

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
        
        // Configure PostgreSQL
        builder.Services.AddDbContext<NetClinicDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // Register custom services
        builder.Services.AddScoped<IOwnerService, OwnerService>();
        builder.Services.AddScoped<IPetService, PetService>();
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
