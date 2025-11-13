using Microsoft.EntityFrameworkCore;
using NetClinic.Api.Data;
using NetClinic.Api.Dto;
using NetClinic.Api.Models;

namespace NetClinic.Api.Services;

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

    public async Task<IEnumerable<OwnerDto>> GetAllOwnersAsync(string? lastName = null)
    {
        _logger.LogInformation("Fetching all owners from the database");

        string lastNameFilter = lastName ?? string.Empty;
        var owners = await _context.Owners.ToListAsync();
        var ownerDtos = owners
            .Where(o => o.LastName.StartsWith(lastNameFilter, StringComparison.OrdinalIgnoreCase))
            .Select(owner => MapOwnerToOwnerDto(owner)).ToList();

        _logger.LogInformation("Successfully fetched {Count} owners", ownerDtos.Count);

        return ownerDtos;
    }

    static OwnerDto MapOwnerToOwnerDto(Owner owner)
    {
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
