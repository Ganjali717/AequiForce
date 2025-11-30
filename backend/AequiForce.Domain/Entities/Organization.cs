using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AequiForce.Domain.Entities;

[Table("organizations")]
public class Organization
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [MaxLength(256)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [MaxLength(256)]
    [Column("legal_name")]
    public string? LegalName { get; set; }

    [MaxLength(2)]
    [Column("country_code")]
    public string? CountryCode { get; set; } // ISO-3166-1 alpha-2

    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}