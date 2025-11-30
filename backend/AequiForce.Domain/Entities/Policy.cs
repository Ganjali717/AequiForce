using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AequiForce.Domain.Entities;

[Table("policies")]
public class Policy
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("organization_id")]
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(3)]
    [Column("base_currency")]
    public string BaseCurrency { get; set; } = "USD"; // ISO-4217

    [Column("is_default")]
    public bool IsDefault { get; set; }

    [Column("effective_from")]
    public DateTimeOffset EffectiveFrom { get; set; } = DateTimeOffset.UtcNow;

    [Column("effective_to")]
    public DateTimeOffset? EffectiveTo { get; set; }

    /// <summary>
    /// Базовый % реинвестиций, если не сработало ни одно правило.
    /// 0..100
    /// </summary>
    [Range(0, 100)]
    [Column("default_reinvestment_percent")]
    public decimal DefaultReinvestmentPercent { get; set; }

    public ICollection<PolicyRule> Rules { get; set; } = new List<PolicyRule>();
    public ICollection<PolicyAllocation> Allocations { get; set; } = new List<PolicyAllocation>();
}