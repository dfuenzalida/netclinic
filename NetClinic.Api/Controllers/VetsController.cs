using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Dto;
using NetClinic.Api.Services;

namespace NetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VetsController(ILogger<VetsController> logger, IVetService vetService, TimeProvider timeProvider) : ControllerBase
{
    private const int PageSize = 5;

    [HttpGet]
    public async Task<VetListDto> Get([FromQuery] int page = 1)
    {
        logger.LogInformation("Vets GET request received at {Timestamp}", timeProvider.GetUtcNow());
        
        try
        {
            var totalVets = await vetService.GetVeterinariansCountAsync();
            var totalPages = (int)Math.Ceiling(totalVets / (double)PageSize);

            var veterinarians = await vetService.GetAllVeterinariansAsync(page);
            var vetList = new VetListDto
            {
                VetList = veterinarians.ToList(),
                TotalPages = totalPages
            };
            logger.LogInformation("Successfully retrieved {Count} veterinarians", veterinarians.Count());
            return vetList;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving veterinarians");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VetDto>> GetById(int id)
    {
        logger.LogInformation("Vets GET by ID request received for ID: {Id} at {Timestamp}", id, timeProvider.GetUtcNow());
        
        try
        {
            var veterinarian = await vetService.GetVeterinarianByIdAsync(id);
            
            if (veterinarian == null)
            {
                logger.LogWarning("Veterinarian with ID {Id} not found", id);
                return NotFound();
            }

            logger.LogInformation("Successfully retrieved veterinarian with ID: {Id}", id);
            return veterinarian;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving veterinarian with ID: {Id}", id);
            throw;
        }
    }
}