using System.ComponentModel.DataAnnotations.Schema;

namespace NetClinic.Api.Models;

[Table("visits")]
public class Visit
{
    [Column("id")]
    public int Id { get; set; }

    [Column("visit_date")]
    public DateTime VisitDate { get; set; }

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("pet_id")]
    public int PetId { get; set; }

    public Pet? Pet { get; set; }
}