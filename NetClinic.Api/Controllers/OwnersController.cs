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
            var owners = await _ownerService.GetAllOwnersAsync(lastName, page, pageSize);
            var ownerList = new OwnerListDto
            {
                OwnerList = owners.ToList()
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
    public async Task<ActionResult<OwnerDto>> CreateOwner([FromBody] OwnerDto ownerDto)
    {
        _logger.LogInformation("Owner POST request received at {Timestamp}", DateTime.UtcNow);

        try
        {
            var createdOwner = await _ownerService.CreateOwnerAsync(ownerDto);
            _logger.LogInformation("Successfully created owner with ID {OwnerId}", createdOwner.Id);
            return CreatedAtAction(nameof(CreateOwner), new { ownerId = createdOwner.Id }, createdOwner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a new owner");
            throw;
        }
    }
}