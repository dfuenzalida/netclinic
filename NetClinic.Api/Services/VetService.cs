using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public interface IVetService
{
    Task<IEnumerable<VetDto>> GetAllVeterinariansAsync();
    Task<VetDto?> GetVeterinarianByIdAsync(int id);
}

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

    public async Task<IEnumerable<VetDto>> GetAllVeterinariansAsync()
    {
        _logger.LogDebug("Retrieving all veterinarians from database");

        try
        {
            var veterinarians = await _context.Veterinarians.Include(v => v.Specialties).ToListAsync();
            var vetDtoList = veterinarians.Select(v => MapVeterinariansToDtos(v)).ToList();
            _logger.LogInformation("Successfully retrieved {Count} veterinarians from database", veterinarians.Count);
            return vetDtoList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarians from database");
            throw;
        }
    }


    public async Task<VetDto?> GetVeterinarianByIdAsync(int id)
    {
        _logger.LogDebug("Retrieving veterinarian with ID: {Id} from database", id);

        try
        {
            var vet = await _context.Veterinarians.FindAsync(id);

            if (vet == null)
            {
                _logger.LogWarning("Veterinarian with ID {Id} not found in database", id);
                return null;
            }
            else
            {
                _logger.LogInformation("Successfully retrieved veterinarian with ID: {Id} from database", id);
                var specialties = vet.Specialties;

                return new VetDto
                {
                    Id = vet.Id,
                    FirstName = vet.FirstName,
                    LastName = vet.LastName,
                    Specialties = specialties.Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList()
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarian with ID: {Id} from database", id);
            throw;
        }
    }

    internal VetDto MapVeterinariansToDtos(Veterinarian veterinarian)
    {
        var specialties = veterinarian.Specialties;

        return new VetDto
        {
            Id = veterinarian.Id,
            FirstName = veterinarian.FirstName,
            LastName = veterinarian.LastName,
            Specialties = specialties.Select(s => new SpecialtyDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList()
        };
    }
}