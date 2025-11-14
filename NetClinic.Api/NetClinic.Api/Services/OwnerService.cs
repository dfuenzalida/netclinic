using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

public interface IOwnerService
{
    Task<IEnumerable<OwnerDto>> GetAllOwnersAsync(string? lastName = null, int page = 1);
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

    // TODO add pagination support
    public async Task<IEnumerable<OwnerDto>> GetAllOwnersAsync(string? lastName = null, int page = 1)
    {
        _logger.LogInformation("Fetching owners from the database with lastName filter: {LastNameFilter}", lastName ?? "none");

        var query = _context.Owners.AsQueryable();

        if (!string.IsNullOrEmpty(lastName))
        {
            query = query.Where(o => o.LastName.StartsWith(lastName));
        }

        var owners = await query.Include(o => o.Pets).ToListAsync();
        var ownerDtos = owners.Select(owner => MapOwnerToOwnerDto(owner)).ToList();

        _logger.LogInformation("Successfully fetched {Count} owners", ownerDtos.Count);

        return ownerDtos;
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
