using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OwnersController : ControllerBase
{
    private readonly ILogger<OwnersController> _logger;
    private readonly IOwnerService _ownerService;

    public OwnersController(
        ILogger<OwnersController> logger,
        IOwnerService ownerService)
    {
        _logger = logger;
        _ownerService = ownerService;
    }

    [HttpGet]
    public async Task<OwnerListDto> Get([FromQuery] string? lastName = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        _logger.LogInformation("Owners GET request received at {Timestamp} for page {Page}", DateTime.UtcNow, page);

        try
        {
            var totalOwners = await _ownerService.GetOwnersByLastNameCountAsync(lastName);
            var totalPages = (int) Math.Floor(totalOwners / (double)pageSize) + (totalOwners % pageSize == 0 ? 0 : 1);

            var owners = await _ownerService.GetOwnersByLastNameAsync(lastName, page, pageSize);
            var ownerList = new OwnerListDto
            {
                OwnerList = owners.ToList(),
                TotalPages = totalPages
            };
            _logger.LogInformation("Successfully retrieved {Count} owners", owners.Count());
            return ownerList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving owners");
            throw;
        }
    }

    [HttpGet("{ownerId}")]
    public async Task<ActionResult<OwnerDetailsDto>> GetOwnerDetailsById([FromRoute] int ownerId)
    {
        _logger.LogInformation("Owner GET by ID request received at {Timestamp} for Owner ID {OwnerId}", DateTime.UtcNow, ownerId);

        try
        {
            var owner = await _ownerService.GetOwnerDetailsByIdAsync(ownerId);
            if (owner == null)
            {
                _logger.LogWarning("Owner with ID {OwnerId} not found", ownerId);
                return NotFound();
            }
            
            _logger.LogInformation("Successfully retrieved owner with ID {OwnerId}", ownerId);
            return owner;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving owner with ID {OwnerId}", ownerId);
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<OwnerDto>> CreateOwner([FromBody] OwnerDto newOwnerDto)
    {
        _logger.LogInformation("Owner POST request received at {Timestamp}", DateTime.UtcNow);

        var errors = ValidateOwnerDto(newOwnerDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var createdOwner = await _ownerService.CreateOwnerAsync(newOwnerDto);
            _logger.LogInformation("Successfully created owner with ID {OwnerId}", createdOwner.Id);
            return CreatedAtAction(nameof(CreateOwner), new { ownerId = createdOwner.Id }, createdOwner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a new Owner");
            throw;
        }
    }

    public static Dictionary<string, string> ValidateOwnerDto(OwnerDto? ownerDto)
    {
        var errors = new Dictionary<string, string>();

        if (ownerDto == null)
        {
            errors.Add("owner", "Owner data is required.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(ownerDto.FirstName))
        {
            errors.Add("firstName", "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.LastName))
        {
            errors.Add("lastName", "Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.Address))
        {
            errors.Add("address", "Address is required.");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.City))
        {
            errors.Add("city", "City is required.");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.Telephone))
        {
            errors.Add("telephone", "Telephone is required.");
        }

        return errors;
    }
}