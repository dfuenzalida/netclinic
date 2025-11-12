using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Services;

namespace NetClinic.Api;

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
    public IEnumerable<Veterinarian> Get()
    {
        _logger.LogInformation("Vets GET request received at {Timestamp}", DateTime.UtcNow);
        
        try
        {
            var veterinarians = _vetService.GetAllVeterinarians();
            _logger.LogInformation("Successfully retrieved {Count} veterinarians", veterinarians.Count());
            return veterinarians;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving veterinarians");
            throw;
        }
    }
}