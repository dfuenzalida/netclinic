namespace NetClinic.Api.Dto;

public class VetDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<SpecialtyDto> Specialties { get; set; } = new();
}