using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VetsController : ControllerBase
{
    private readonly ILogger<VetsController> _logger;
    private readonly IVetService _vetService;

    public VetsController(
        ILogger<VetsController> logger,
        IVetService vetService)
    {
        _logger = logger;
        _vetService = vetService;
    }

    [HttpGet]
    public async Task<VetListDto> Get()
    {
        _logger.LogInformation("Vets GET request received at {Timestamp}", DateTime.UtcNow);
        
        try
        {
            var veterinarians = await _vetService.GetAllVeterinariansAsync();
            var vetList = new VetListDto
            {
                VetList = veterinarians.ToList()
            };
            _logger.LogInformation("Successfully retrieved {Count} veterinarians", veterinarians.Count());
            return vetList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarians");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VetDto>> GetById(int id)
    {
        _logger.LogInformation("Vets GET by ID request received for ID: {Id} at {Timestamp}", id, DateTime.UtcNow);
        
        try
        {
            var veterinarian = await _vetService.GetVeterinarianByIdAsync(id);
            
            if (veterinarian == null)
            {
                _logger.LogWarning("Veterinarian with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved veterinarian with ID: {Id}", id);
            return veterinarian;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarian with ID: {Id}", id);
            throw;
        }
    }
}