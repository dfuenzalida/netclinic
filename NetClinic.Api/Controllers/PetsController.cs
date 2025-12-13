using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("owners/{ownerId}/[controller]")]
public class PetsController(ILogger<PetsController> logger, IPetService petService) : ControllerBase
{
    private readonly ILogger<PetsController> _logger = logger;
    private readonly IPetService _petService = petService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetPetsByOwnerId([FromRoute] int ownerId)
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

    [HttpGet("{petId}")]
    public async Task<ActionResult<PetDto?>> GetPetById([FromRoute] int ownerId, [FromRoute] int petId)
    {
        _logger.LogInformation("Pet GET by ID request received at {Timestamp} for Pet ID {PetId}", DateTime.UtcNow, petId);

        try
        {
            var pet = await _petService.GetPetByIdAsync(petId);
            if (pet == null)
            {
                _logger.LogWarning("No pet found for Pet ID {PetId}", petId);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved pet for Pet ID {PetId}", petId);
            return Ok(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving pet for Pet ID {PetId}", petId);
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
    
    [HttpPost]
    public async Task<ActionResult<PetDto>> CreatePet([FromRoute] int ownerId, [FromBody] PetDto newPetDto)
    {
        _logger.LogInformation("Pet POST request received at {Timestamp}", DateTime.UtcNow);

        var errors = ValidatePetDto(newPetDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var createdPet = await _petService.CreatePetAsync(newPetDto, ownerId);
            _logger.LogInformation("Successfully created pet with ID {PetId}", createdPet.Id);
            return CreatedAtAction(nameof(CreatePet), new { petId = createdPet.Id }, createdPet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a new Pet");
            throw;
        }
    }

    [HttpPut("{petId}")]
    public async Task<ActionResult<PetDto?>> UpdatePet([FromRoute] int ownerId, [FromRoute] int petId, [FromBody] PetDto petDto)
    {
        _logger.LogInformation("Pet PUT request received at {Timestamp} for Pet ID {PetId}", DateTime.UtcNow, petId);

        // Use the petId from the route
        petDto.Id = petId;
        var errors = ValidatePetDto(petDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var updatedPet = await _petService.UpdatePetAsync(petDto);
            if (updatedPet == null)
            {
                _logger.LogWarning("No pet found for Pet ID {PetId} to update", petId);
                return NotFound();
            }

            _logger.LogInformation("Successfully updated pet with ID {PetId}", petId);
            return Ok(updatedPet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating Pet ID {PetId}", petId);
            throw;
        }
    }

    [HttpGet("/pet/types")]
    public async Task<ActionResult<IEnumerable<PetTypeDto>>> GetAllPetTypes()
    {
        _logger.LogInformation("Pet Types GET request received at {Timestamp}", DateTime.UtcNow);

        try
        {
            var petTypes = await _petService.GetAllPetTypesAsync();
            _logger.LogInformation("Successfully retrieved {Count} pet types", petTypes.Count());
            return Ok(petTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving pet types");
            throw;
        }
    }

    [HttpPost("{petId}/visits")]
    public async Task<ActionResult<VisitDto>> CreateVisit([FromRoute] int petId, [FromBody] VisitDto newVisitDto)
    {
        _logger.LogInformation("Visit POST request received at {Timestamp}", DateTime.UtcNow);

        var errors = ValidateVisitDto(newVisitDto);

        if (errors.Count != 0)
        {
            return BadRequest(errors);
        }

        try
        {
            var createdVisit = await _petService.CreateVisitAsync(petId, newVisitDto);
            _logger.LogInformation("Successfully created visit with ID {VisitId}", createdVisit.Id);
            return CreatedAtAction(nameof(CreateVisit), new { visitId = createdVisit.Id }, createdVisit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a new Visit");
            throw;
        }
    }

    public static Dictionary<string, string> ValidateVisitDto(VisitDto newVisitDto)
    {
        var errors = new Dictionary<string, string>();

        if (newVisitDto == null)
        {
            errors.Add("visit", "must not be blank");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(newVisitDto.Description))
        {
            errors.Add("description", "must not be blank");
        }

        if (newVisitDto.VisitDate == default)
        {
            errors.Add("visitDate", "must not be blank");
        }

        return errors;
    }

    public static Dictionary<string, string> ValidatePetDto(PetDto petDto)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(petDto.Name))
        {
            errors.Add("name", "is required");
        }

        if (string.IsNullOrWhiteSpace(petDto.Type))
        {
            errors.Add("type", "is required");
        }

        if (string.IsNullOrWhiteSpace(petDto.BirthDate) || !DateTime.TryParse(petDto.BirthDate, out _))
        {
            errors.Add("birthDate", "is required");
        }

        return errors;
    }
}