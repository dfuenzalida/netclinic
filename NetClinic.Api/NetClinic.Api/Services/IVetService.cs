namespace NetClinic.Api.Services;

public interface IVetService
{
    Task<IEnumerable<Veterinarian>> GetAllVeterinariansAsync();
    Task<Veterinarian?> GetVeterinarianByIdAsync(int id);
}