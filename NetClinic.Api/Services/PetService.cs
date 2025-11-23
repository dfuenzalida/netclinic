using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using Microsoft.EntityFrameworkCore;

namespace NetClinic.Api.Services;

public interface IPetService
{
    Task<IEnumerable<PetDto>?> GetPetsByOwnerIdAsync(int ownerId);
    Task<PetDto?> GetPetByIdAsync(int petId);
    Task<IEnumerable<VisitDto>?> GetVisitsByPetIdAsync(int ownerId, int petId);
}

public class PetService : IPetService
{
    private readonly NetClinicDbContext _context;
    private readonly ILogger<PetService> _logger;

    public PetService(NetClinicDbContext context, ILogger<PetService> logger)
    {
        _context = context;
        _logger = logger;
        _logger.LogInformation("PetService initialized with database context");
    }

    public async Task<IEnumerable<PetDto>?> GetPetsByOwnerIdAsync(int ownerId)
    {
        _logger.LogInformation("Fetching pets from the database for Owner ID: {OwnerId}", ownerId);

        var pets = await _context.Pets
                                 .Include(p => p.PetType)
                                 .Where(p => p.OwnerId == ownerId)
                                 .OrderBy(p => p.Name)
                                 .ToListAsync();

        if (pets == null || pets.Count == 0)
        {
            _logger.LogWarning("No pets found for Owner ID: {OwnerId}", ownerId);
            return null;
        }

        var petDtos = pets.Select(pet => new PetDto
        {
            Id = pet.Id,
            Name = pet.Name,
            Type = pet.PetType.Name,
            BirthDate = pet.BirthDate.ToString("yyyy-MM-dd")
        }).ToList();

        _logger.LogInformation("Successfully fetched {Count} pets for Owner ID: {OwnerId}", petDtos.Count, ownerId);

        return petDtos;
    }

    public async Task<PetDto?> GetPetByIdAsync(int petId)
    {
        _logger.LogInformation("Fetching pet from the database for Pet ID: {PetId}", petId);

        var pet =  await _context.Pets
                           .Where(p => p.Id == petId)
                           .Select(pet => new PetDto
                           {
                               Id = pet.Id,
                               Name = pet.Name,
                               Type = pet.PetType.Name,
                               BirthDate = pet.BirthDate.ToString("yyyy-MM-dd")
                           })
                           .FirstOrDefaultAsync();

        if (pet == null)
        {
            _logger.LogWarning("No pet found for Pet ID: {PetId}", petId);
            return null;
        }

        _logger.LogInformation("Successfully fetched pet for Pet ID: {PetId}", petId);

        return pet;
    }

    public async Task<IEnumerable<VisitDto>?> GetVisitsByPetIdAsync(int ownerId, int petId)
    {
        _logger.LogInformation("Fetching visits from the database for Pet ID: {PetId}", petId);

        var visits = await _context.Pets
                                   .Where(p => p.OwnerId == ownerId)
                                   .Where(p => p.Id == petId)
                                   .SelectMany(p => p.Visits)
                                   .OrderBy(v => v.VisitDate)
                                   .Select(visit => new VisitDto
                                   {
                                       Id = visit.Id,
                                       VisitDate = visit.VisitDate.ToString("yyyy-MM-dd"),
                                       Description = visit.Description
                                   })
                                   .ToListAsync();

        if (visits == null || visits.Count == 0)
        {
            _logger.LogWarning("No visits found for Pet ID: {PetId}", petId);
            return null;
        }

        _logger.LogInformation("Successfully fetched {Count} visits for Pet ID: {PetId}", visits.Count, petId);

        return visits;
    }

}