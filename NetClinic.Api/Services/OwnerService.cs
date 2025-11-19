using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public interface IOwnerService
{
    Task<IEnumerable<OwnerDto>> GetAllOwnersAsync(string? lastName = null, int page = 1, int pageSize = 5);

    Task<OwnerDetailsDto?> GetOwnerDetailsByIdAsync(int ownerId);
    
    Task<OwnerDto> CreateOwnerAsync(OwnerDto ownerDto);
}

public class OwnerService : IOwnerService
{
    private readonly NetClinicDbContext _context;
    private readonly ILogger<OwnerService> _logger;

    public OwnerService(NetClinicDbContext context, ILogger<OwnerService> logger)
    {
        _context = context;
        _logger = logger;
        _logger.LogInformation("OwnerService initialized with database context");
    }

    public async Task<IEnumerable<OwnerDto>> GetAllOwnersAsync(string? lastName = null, int page = 1, int pageSize = 5)
    {
        _logger.LogInformation("Fetching owners from the database with lastName filter: {LastNameFilter}", lastName ?? "none");

        var query = _context.Owners.AsQueryable();

        if (!string.IsNullOrEmpty(lastName))
        {
            query = query.Where(o => o.LastName.ToLower().StartsWith(lastName.ToLower()));
        }

        var owners = await query.Include(o => o.Pets)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
        var ownerDtos = owners.Select(owner => MapOwnerToOwnerDto(owner)).ToList();

        _logger.LogInformation("Successfully fetched {Count} owners", ownerDtos.Count);

        return ownerDtos;
    }

    public async Task<OwnerDetailsDto?> GetOwnerDetailsByIdAsync(int ownerId)
    {
        _logger.LogInformation("Fetching owner details for OwnerId: {OwnerId}", ownerId);

        var query = _context.Owners.AsQueryable();

        var owner = await _context.Owners
            .Where(o => o.Id == ownerId)
            .Include(o => o.Pets)
                .ThenInclude(p => p.Visits)
            .Include(o => o.Pets)
                .ThenInclude(p => p.PetType)
            .FirstOrDefaultAsync();

        if (owner == null)
        {
            _logger.LogWarning("Owner with Id {OwnerId} not found", ownerId);
            return null;
        }

        var ownerDetailsDto = new OwnerDetailsDto
        {
            Id = owner.Id,
            FirstName = owner.FirstName,
            LastName = owner.LastName,
            Address = owner.Address,
            City = owner.City,
            Telephone = owner.Telephone,
            Pets = owner.Pets.Select(p => new PetDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.PetType.Name,
                BirthDate = p.BirthDate,
                Visits = p.Visits.Select(v => new VisitDto
                {
                    Id = v.Id,
                    VisitDate = v.VisitDate,
                    Description = v.Description
                }).ToList()
            }).ToList()
        };

        _logger.LogInformation("Successfully fetched details for OwnerId: {OwnerId}", ownerId);

        return ownerDetailsDto;
    }

    public async Task<OwnerDto> CreateOwnerAsync(OwnerDto ownerDto)
    {
        _logger.LogInformation("Creating new owner: {FirstName} {LastName}", ownerDto.FirstName, ownerDto.LastName);

        var owner = new Owner
        {
            FirstName = ownerDto.FirstName,
            LastName = ownerDto.LastName,
            Address = ownerDto.Address,
            City = ownerDto.City,
            Telephone = ownerDto.Telephone
        };

        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully created owner with Id: {OwnerId}", owner.Id);

        // Return the created owner as DTO with the generated ID
        return MapOwnerToOwnerDto(owner);
    }

    static OwnerDto MapOwnerToOwnerDto(Owner owner)
    {
        var pets = owner.Pets.Select(p => p.Name).OrderBy(p => p).ToList();
        return new OwnerDto
        {
            Id = owner.Id,
            FirstName = owner.FirstName,
            LastName = owner.LastName,
            Address = owner.Address,
            City = owner.City,
            Telephone = owner.Telephone,
            Pets = pets
        };
    }
}
