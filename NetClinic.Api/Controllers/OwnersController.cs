using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnersController(ILogger<OwnersController> logger, IOwnerService ownerService) : ControllerBase
{
    private readonly ILogger<OwnersController> _logger = logger;
    private readonly IOwnerService _ownerService = ownerService;

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
    public async Task<ActionResult<OwnerDto>> GetOwnerDetailsById([FromRoute] int ownerId)
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

    [HttpPut("{ownerId}")]
    public async Task<ActionResult<OwnerDto>> UpdateOwner([FromRoute] int ownerId, [FromBody] OwnerDto ownerDto)
    {
        _logger.LogInformation("Owner PUT request received at {Timestamp} for Owner ID {OwnerId}", DateTime.UtcNow, ownerDto.Id);

        ownerDto.Id = ownerId; // Ensure the ID from the route is used
        var errors = ValidateOwnerDto(ownerDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var updatedOwner = await _ownerService.UpdateOwnerAsync(ownerDto);
            if (updatedOwner == null)
            {
                _logger.LogWarning("Owner with ID {OwnerId} not found for update", ownerDto.Id);
                return NotFound();
            }

            _logger.LogInformation("Successfully updated owner with ID {OwnerId}", ownerDto.Id);
            return updatedOwner;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating Owner with ID {OwnerId}", ownerDto.Id);
            throw;
        }
    }

    public static Dictionary<string, string> ValidateOwnerDto(OwnerDto? ownerDto)
    {
        var errors = new Dictionary<string, string>();

        if (ownerDto == null)
        {
            errors.Add("owner", "must not be blank");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(ownerDto.FirstName))
        {
            errors.Add("firstName", "must not be blank");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.LastName))
        {
            errors.Add("lastName", "must not be blank");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.Address))
        {
            errors.Add("address", "must not be blank");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.City))
        {
            errors.Add("city", "must not be blank");
        }

        if (string.IsNullOrWhiteSpace(ownerDto.Telephone))
        {
            errors.Add("telephone", "must not be blank");
        }
        else
        {
            if (ownerDto.Telephone.Length != 10 || !long.TryParse(ownerDto.Telephone, out _))
            {
                errors.Add("telephone", "Telephone must be a 10-digit number");
            }
        }

        return errors;
    }
}