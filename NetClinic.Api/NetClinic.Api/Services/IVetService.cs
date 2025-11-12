namespace NetClinic.Api.Services;

public interface IVetService
{
    IEnumerable<Veterinarian> GetAllVeterinarians();
}