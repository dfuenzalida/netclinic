using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public interface IPetService
{
    Task<IEnumerable<PetDto>?> GetPetsByOwnerIdAsync(int ownerId);
    Task<PetDto?> GetPetByIdAsync(int petId);
    Task<IEnumerable<VisitDto>?> GetVisitsByPetIdAsync(int ownerId, int petId);
    Task<PetDto> CreatePetAsync(PetDto petDto, int ownerId);
    Task<PetDto?> UpdatePetAsync(PetDto petDto);
    Task<IEnumerable<PetTypeDto>> GetAllPetTypesAsync();
}

public class PetService : IPetService
{
    private readonly NetClinicDbContext _context;
    private readonly ILogger<PetService> _logger;

    public static PetType UnknownPetType = new PetType
    {
        Id = 0,
        Name = "unknown"
    };


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

    public async Task<IEnumerable<PetTypeDto>> GetAllPetTypesAsync()
    {
        _logger.LogInformation("Fetching all pet types from the database");

        var petTypes = await _context.PetTypes
                                     .OrderBy(pt => pt.Name)
                                     .ToListAsync();

        var petTypeDtos = petTypes.Select(pt => MapPetTypeToPetTypeDto(pt)).ToList();

        _logger.LogInformation("Successfully fetched {Count} pet types", petTypeDtos.Count);

        return petTypeDtos;
    }

    public async Task<PetDto> CreatePetAsync(PetDto petDto, int ownerId)
    {
        // Implementation for creating a new pet
        var pet = new Pet
        {
            Name = petDto.Name,
            BirthDate = DateTime.SpecifyKind(DateTime.Parse(petDto.BirthDate), DateTimeKind.Utc),
            PetType = await _context.PetTypes
                                  .Where(pt => pt.Name == petDto.Type)
                                  .FirstOrDefaultAsync() ?? UnknownPetType,
            OwnerId = ownerId
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Successfully created a new pet with ID: {PetId}", pet.Id);

        return MapPetToPetDto(pet);
    }

    public async Task<PetDto?> UpdatePetAsync(PetDto petDto)
    {
        var pet = await _context.Pets.FindAsync(petDto.Id);
        if (pet == null)
        {
            _logger.LogWarning("No pet found for ID: {PetId}", petDto.Id);
            return null;
        }

        pet.Name = petDto.Name;
        pet.BirthDate = DateTime.SpecifyKind(DateTime.Parse(petDto.BirthDate), DateTimeKind.Utc);
        pet.PetType = await _context.PetTypes
                                    .Where(pt => pt.Name == petDto.Type)
                                    .FirstOrDefaultAsync() ?? UnknownPetType;

        _context.Pets.Update(pet);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Successfully updated pet with ID: {PetId}", pet.Id);

        return MapPetToPetDto(pet);
    }

    public static PetDto MapPetToPetDto(Pet pet)
    {
        return new PetDto
        {
            Id = pet.Id,
            Name = pet.Name,
            Type = pet.PetType.Name,
            BirthDate = pet.BirthDate.ToString("yyyy-MM-dd")
        };
    }

    public static PetTypeDto MapPetTypeToPetTypeDto(PetType petType)
    {
        return new PetTypeDto
        {
            Id = petType.Id,
            Name = petType.Name
        };
    }
}