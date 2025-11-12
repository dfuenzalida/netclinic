using NetClinic.Api.Dto;

namespace NetClinic.Api.Services;

public interface IVetService
{
    Task<IEnumerable<VetDto>> GetAllVeterinariansAsync();
    Task<VetDto?> GetVeterinarianByIdAsync(int id);
}