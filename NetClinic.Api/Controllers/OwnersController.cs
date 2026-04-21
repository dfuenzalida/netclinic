using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnersController(ILogger<OwnersController> logger, IOwnerService ownerService, TimeProvider timeProvider) : ControllerBase
{

    [HttpGet]
    public async Task<OwnerListDto> Get([FromQuery] string? lastName = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        logger.LogInformation("Owners GET request received at {Timestamp} for page {Page}", timeProvider.GetUtcNow(), page);

        try
        {
            var totalOwners = await ownerService.GetOwnersByLastNameCountAsync(lastName);
            var totalPages = (int)Math.Ceiling(totalOwners / (double)pageSize);

            var owners = await ownerService.GetOwnersByLastNameAsync(lastName, page, pageSize);
            var ownerList = new OwnerListDto
            {
                OwnerList = owners.ToList(),
                TotalPages = totalPages
            };
            logger.LogInformation("Successfully retrieved {Count} owners", owners.Count());
            return ownerList;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving owners");
            throw;
        }
    }

    [HttpGet("{ownerId}")]
    public async Task<ActionResult<OwnerDto>> GetOwnerDetailsById([FromRoute] int ownerId)
    {
        logger.LogInformation("Owner GET by ID request received at {Timestamp} for Owner ID {OwnerId}", timeProvider.GetUtcNow(), ownerId);

        try
        {
            var owner = await ownerService.GetOwnerDetailsByIdAsync(ownerId);
            if (owner == null)
            {
                logger.LogWarning("Owner with ID {OwnerId} not found", ownerId);
                return NotFound();
            }
            
            logger.LogInformation("Successfully retrieved owner with ID {OwnerId}", ownerId);
            return owner;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving owner with ID {OwnerId}", ownerId);
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<OwnerDto>> CreateOwner([FromBody] OwnerDto newOwnerDto)
    {
        logger.LogInformation("Owner POST request received at {Timestamp}", timeProvider.GetUtcNow());

        var errors = ValidateOwnerDto(newOwnerDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var createdOwner = await ownerService.CreateOwnerAsync(newOwnerDto);
            logger.LogInformation("Successfully created owner with ID {OwnerId}", createdOwner.Id);
            return CreatedAtAction(nameof(CreateOwner), new { ownerId = createdOwner.Id }, createdOwner);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating a new Owner");
            throw;
        }
    }

    [HttpPut("{ownerId}")]
    public async Task<ActionResult<OwnerDto>> UpdateOwner([FromRoute] int ownerId, [FromBody] OwnerDto ownerDto)
    {
        logger.LogInformation("Owner PUT request received at {Timestamp} for Owner ID {OwnerId}", timeProvider.GetUtcNow(), ownerDto.Id);

        ownerDto.Id = ownerId; // Ensure the ID from the route is used
        var errors = ValidateOwnerDto(ownerDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var updatedOwner = await ownerService.UpdateOwnerAsync(ownerDto);
            if (updatedOwner == null)
            {
                logger.LogWarning("Owner with ID {OwnerId} not found for update", ownerDto.Id);
                return NotFound();
            }

            logger.LogInformation("Successfully updated owner with ID {OwnerId}", ownerDto.Id);
            return updatedOwner;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating Owner with ID {OwnerId}", ownerDto.Id);
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