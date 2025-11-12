using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public class VetService : IVetService
{
    private readonly NetClinicDbContext _context;
    private readonly ILogger<VetService> _logger;

    public VetService(NetClinicDbContext context, ILogger<VetService> logger)
    {
        _context = context;
        _logger = logger;
        _logger.LogInformation("VetService initialized with database context");
    }

    public async Task<IEnumerable<Veterinarian>> GetAllVeterinariansAsync()
    {
        _logger.LogDebug("Retrieving all veterinarians from database");

        try
        {
            var veterinarians = await _context.Veterinarians.ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} veterinarians from database", veterinarians.Count);
            return veterinarians;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarians from database");
            throw;
        }
    }

    public async Task<Veterinarian?> GetVeterinarianByIdAsync(int id)
    {
        _logger.LogDebug("Retrieving veterinarian with ID: {Id} from database", id);

        try
        {
            var veterinarian = await _context.Veterinarians.FindAsync(id);

            if (veterinarian == null)
            {
                _logger.LogWarning("Veterinarian with ID {Id} not found in database", id);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved veterinarian with ID: {Id} from database", id);
            }

            return veterinarian;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarian with ID: {Id} from database", id);
            throw;
        }
    }
}