namespace NetClinic.Api.Services;

public class VetService : IVetService
{
    private readonly List<Veterinarian> _veterinarians;
    private readonly ILogger<VetService> _logger;

    public VetService(ILogger<VetService> logger)
    {
        _logger = logger;
        _veterinarians = new List<Veterinarian>
        {
            new(1, "Sarah", "Johnson"),
            new(2, "Michael", "Chen"),
            new(3, "Emily", "Rodriguez"),
            new(4, "David", "Thompson"),
            new(5, "Lisa", "Anderson"),
            new(6, "Robert", "Wilson")
        };

        _logger.LogInformation("VetService initialized with {Count} veterinarians", _veterinarians.Count);
    }

    public IEnumerable<Veterinarian> GetAllVeterinarians()
    {
        _logger.LogDebug("Retrieving all veterinarians");
        return _veterinarians;
    }
}