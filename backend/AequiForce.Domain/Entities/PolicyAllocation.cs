using AequiForce.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AequiForce.Domain.Entities;

[Table("policy_allocations")]
public class PolicyAllocation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("policy_id")]
    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    [Column("bucket")]
    public AllocationBucket Bucket { get; set; }

    /// <summary>
    /// Доля от реинвестируемой суммы для данного "кармана", 0..100.
    /// Сумма по Policy, как правило, <= 100.
    /// </summary>
    [Range(0, 100)]
    [Column("percentage")]
    public decimal Percentage { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}