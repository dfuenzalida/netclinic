namespace NetClinic.Api.Dto;

public class OwnerDetailsDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;

    // Collection of of Pet details
    public IEnumerable<PetDetailsDto> Pets { get; set; } = [];
}

public class PetDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }

    // Collection of Visits
    public IEnumerable<VisitDto> Visits { get; set; } = [];
}

public class VisitDto
{
    public int Id { get; set; }
    public DateTime VisitDate { get; set; }
    public string Description { get; set; } = string.Empty;
}