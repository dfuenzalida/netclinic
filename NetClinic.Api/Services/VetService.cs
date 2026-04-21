using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public interface IVetService
{
    Task<IEnumerable<VetDto>> GetAllVeterinariansAsync(int page = 1, int pageSize = 5);
    Task<int> GetVeterinariansCountAsync();
    Task<VetDto?> GetVeterinarianByIdAsync(int id);
}

public class VetService(NetClinicDbContext context, ILogger<VetService> logger) : IVetService
{

    public async Task<IEnumerable<VetDto>> GetAllVeterinariansAsync(int page = 1, int pageSize = 5)
    {
        logger.LogDebug("Retrieving all veterinarians from database");

        try
        {
            var veterinarians = await context.Veterinarians.Include(v => v.Specialties)
                                        .OrderBy(v => v.Id)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            var vetDtoList = veterinarians.Select(v => MapVeterinariansToDtos(v)).ToList();
            logger.LogInformation("Successfully retrieved {Count} veterinarians from database", veterinarians.Count);
            return vetDtoList;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving veterinarians from database");
            throw;
        }
    }

    public async Task<int> GetVeterinariansCountAsync()
    {
        logger.LogDebug("Counting total number of veterinarians in database");

        try
        {
            var count = await context.Veterinarians.CountAsync();
            logger.LogInformation("Total number of veterinarians: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while counting veterinarians in database");
            throw;
        }
    }

    public async Task<VetDto?> GetVeterinarianByIdAsync(int id)
    {
        logger.LogDebug("Retrieving veterinarian with ID: {Id} from database", id);

        try
        {
            var vet = await context.Veterinarians.FindAsync(id);

            if (vet == null)
            {
                logger.LogWarning("Veterinarian with ID {Id} not found in database", id);
                return null;
            }
            else
            {
                logger.LogInformation("Successfully retrieved veterinarian with ID: {Id} from database", id);
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
            logger.LogError(ex, "Error occurred while retrieving veterinarian with ID: {Id} from database", id);
            throw;
        }
    }

    internal static VetDto MapVeterinariansToDtos(Veterinarian veterinarian)
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
            }).OrderBy(s => s.Name).ToList()
        };
    }
}