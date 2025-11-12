using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetClinic.Api.Services;

[Table("vets")]
public class Veterinarian
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("first_name")]
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Column("last_name")]
    [Required]
    public string LastName { get; set; } = string.Empty;

    // Parameterless constructor for EF Core
    public Veterinarian()
    {
    }

    // Constructor with parameters for convenience
    public Veterinarian(int id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
}