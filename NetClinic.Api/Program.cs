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
        
        // Configure PostgreSQL - prioritize Azure connection string if available
        var connectionString = builder.Configuration.GetConnectionString("AZURE_POSTGRESQL_CONNECTIONSTRING") 
                             ?? builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<NetClinicDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        // Add memory caching
        builder.Services.AddMemoryCache();

        builder.Services.AddResponseCompression();
        
        // Register custom services
        builder.Services.AddScoped<IOwnerService, OwnerService>();
        builder.Services.AddScoped<IPetService, PetService>();
        builder.Services.AddScoped<IVetService, VetService>();

        // CORS for the frontend application
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Add request logging middleware
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
            app.MapOpenApi("/api/openapi/v1.json");
        //}

        // Won't use HTTPS directly
        // app.UseHttpsRedirection();

        app.UseResponseCompression();

        app.MapControllers();

        app.Run();
    }
}



