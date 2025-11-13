using NetClinic.Api.Dto;

namespace NetClinic.Api.Services;

public interface IOwnerService
{
    Task<IEnumerable<OwnerDto>> GetAllOwnersAsync(string? lastName = null);
}
