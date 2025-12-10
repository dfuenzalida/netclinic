namespace NetClinic.Api.Dto;

public class PetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public required string BirthDate { get; set; }
}

public class VisitDto
{
    public int Id { get; set; }
    public required string VisitDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class PetTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
