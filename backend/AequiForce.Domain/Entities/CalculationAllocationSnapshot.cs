using AequiForce.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AequiForce.Domain.Entities;

[Table("calculation_allocation_snapshots")]
public class CalculationAllocationSnapshot
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("calculation_snapshot_id")]
    public Guid CalculationSnapshotId { get; set; }
    public CalculationSnapshot CalculationSnapshot { get; set; } = null!;
    
    [Column("bucket")]
    public AllocationBucket Bucket { get; set; }

    /// <summary>
    /// % от реинвестируемой суммы, пришедшийся на этот карман.
    /// </summary>
    [Column("bucket_percent", TypeName = "numeric(5,2)")]
    public decimal BucketPercent { get; set; }

    /// <summary>
    /// Денежная сумма для этого кармана.
    /// </summary>
    [Column("amount", TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }
}