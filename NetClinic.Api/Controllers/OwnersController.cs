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
    public async Task<OwnerListDto> Get([FromQuery] string? lastName = null, [FromQuery] int page = 1)
    {
        // TODO implement pagination using 'page' parameter
        _logger.LogInformation("Owners GET request received at {Timestamp} for page {Page}", DateTime.UtcNow, page);

        try
        {
            var owners = await _ownerService.GetAllOwnersAsync(lastName);
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
}