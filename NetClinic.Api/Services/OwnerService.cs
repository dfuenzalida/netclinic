using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public interface IOwnerService
{
    Task<IEnumerable<OwnerDto>> GetOwnersByLastNameAsync(string? lastName = null, int page = 1, int pageSize = 5);

    Task<int> GetOwnersByLastNameCountAsync(string? lastName = null);

    Task<OwnerDto?> GetOwnerDetailsByIdAsync(int ownerId);
    
    Task<OwnerDto> CreateOwnerAsync(OwnerDto ownerDto);

    Task<OwnerDto?> UpdateOwnerAsync(OwnerDto ownerDto);
}

public class OwnerService(NetClinicDbContext context, ILogger<OwnerService> logger) : IOwnerService
{
    private readonly NetClinicDbContext _context = context;
    private readonly ILogger<OwnerService> _logger = logger;

    public async Task<IEnumerable<OwnerDto>> GetOwnersByLastNameAsync(string? lastName = null, int page = 1, int pageSize = 5)
    {
        _logger.LogInformation("Fetching owners from the database with lastName filter: {LastNameFilter}", lastName ?? "none");

        var query = _context.Owners.AsQueryable();

        if (!string.IsNullOrEmpty(lastName))
        {
            query = query.Where(o => o.LastName.ToLower().StartsWith(lastName.ToLower()));
        }

        var owners = await query
                            .OrderBy(o => o.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        var ownerDtos = owners.Select(owner => MapOwnerToOwnerDto(owner)).ToList();

        _logger.LogInformation("Successfully fetched {Count} owners", ownerDtos.Count);

        return ownerDtos;
    }

    public async Task<int> GetOwnersByLastNameCountAsync(string? lastName = null)
    {
        _logger.LogInformation("Fetching owners from the database with lastName filter: {LastNameFilter}", lastName ?? "none");

        var query = _context.Owners.AsQueryable();

        if (!string.IsNullOrEmpty(lastName))
        {
            query = query.Where(o => o.LastName.ToLower().StartsWith(lastName.ToLower()));
        }

        int ownerCount = await query.CountAsync();

        _logger.LogInformation("Successfully found {Count} owners", ownerCount);

        return ownerCount;
    }

    public async Task<OwnerDto?> GetOwnerDetailsByIdAsync(int ownerId)
    {
        _logger.LogInformation("Fetching owner details for OwnerId: {OwnerId}", ownerId);

        var query = _context.Owners.AsQueryable();

        var owner = await _context.Owners
            .Where(o => o.Id == ownerId)
            .FirstOrDefaultAsync();

        if (owner == null)
        {
            _logger.LogWarning("Owner with Id {OwnerId} not found", ownerId);
            return null;
        }

        var ownerDetailsDto = MapOwnerToOwnerDto(owner);

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

    public async Task<OwnerDto?> UpdateOwnerAsync(OwnerDto ownerDto)
    {
        _logger.LogInformation("Updating owner with Id: {OwnerId}", ownerDto.Id);

        var owner = await _context.Owners.FindAsync(ownerDto.Id);
        if (owner == null)
        {
            _logger.LogWarning("Owner with Id {OwnerId} not found for update", ownerDto.Id);
            return null;
        }

        owner.FirstName = ownerDto.FirstName;
        owner.LastName = ownerDto.LastName;
        owner.Address = ownerDto.Address;
        owner.City = ownerDto.City;
        owner.Telephone = ownerDto.Telephone;

        _context.Owners.Update(owner);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully updated owner with Id: {OwnerId}", owner.Id);

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
            Telephone = owner.Telephone
        };
    }
}
