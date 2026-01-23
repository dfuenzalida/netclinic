using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetClinic.Api.Models;

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

    // Navigation property for many-to-many relationship with Specialty
    public ICollection<Specialty> Specialties { get; set; } = [];

    // Parameterless constructor for EF Core
    public Veterinarian()
    {
    }
}