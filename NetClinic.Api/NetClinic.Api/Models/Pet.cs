using System.ComponentModel.DataAnnotations.Schema;

namespace NetClinic.Api.Models;

[Table("pets")]
public class Pet
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("birth_date")]
    public DateTime BirthDate { get; set; }

    [Column("type_id")]
    public int TypeId { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }
    public Owner? Owner { get; set; }
}
