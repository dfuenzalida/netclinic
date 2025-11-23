using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("owners/{ownerId}/[controller]")]
public class PetsController : ControllerBase
{
    private readonly ILogger<PetsController> _logger;
    private readonly IPetService _petService;

    public PetsController(
        ILogger<PetsController> logger,
        IPetService petService)
    {
        _logger = logger;
        _petService = petService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetDetailsDto>>> GetPetsByOwnerId([FromRoute] int ownerId)
    {
        _logger.LogInformation("Pets GET request received at {Timestamp} for Owner ID {OwnerId}", DateTime.UtcNow, ownerId);

        try
        {
            var pets = await _petService.GetPetsByOwnerIdAsync(ownerId);
            if (pets == null || !pets.Any())
            {
                _logger.LogWarning("No pets found for Owner ID {OwnerId}", ownerId);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved {Count} pets for Owner ID {OwnerId}", pets.Count(), ownerId);
            return Ok(pets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving pets for Owner ID {OwnerId}", ownerId);
            throw;
        }
    }

    [HttpGet("{petId}/visits")]
    public async Task<ActionResult<IEnumerable<VisitDto>?>> GetVisitsByPetId([FromRoute] int ownerId, [FromRoute] int petId)
    {
        _logger.LogInformation("Pet GET by ID request received at {Timestamp} for Pet ID {PetId}", DateTime.UtcNow, petId);

        try
        {
            var visits = await _petService.GetVisitsByPetIdAsync(ownerId, petId);
            if (visits == null || !visits.Any())
            {
                _logger.LogWarning("No visits found for Pet ID {PetId}", petId);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved {Count} visits for Pet ID {PetId}", visits.Count(), petId);
            return Ok(visits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving visits for Pet ID {PetId}", petId);
            throw;
        }
    }
}