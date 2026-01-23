using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetClinic.Api.Models;

[Table("specialties")]
public class Specialty
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    [Required]
    public string Name { get; set; } = string.Empty;

    // Navigation property for many-to-many relationship with Veterinarian
    public ICollection<Veterinarian> Veterinarians { get; set; } = [];

    // Parameterless constructor for EF Core
    public Specialty()
    {
    }
}