namespace NetClinic.Api.Dto;

public class OwnerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;

    // List of Pet names
    public List<string> Pets { get; set; } = new();
}