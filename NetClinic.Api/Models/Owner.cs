using System.ComponentModel.DataAnnotations.Schema;

namespace NetClinic.Api.Models;

[Table("owners")]
public class Owner
{
   [Column("id")]
     public int Id { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("address")]
    public string Address { get; set; } = string.Empty;

    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Column("telephone")]
    public string Telephone { get; set; } = string.Empty;

    public ICollection<Pet> Pets { get; set; } = [];
}