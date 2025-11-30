using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AequiForce.Domain.Entities;

[Table("calculation_snapshots")]
public class CalculationSnapshot
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("project_id")]
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    [Required]
    [Column("policy_id")]
    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    [Column("calculated_at")]
    public DateTimeOffset CalculatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("performed_by_user_id")]
    public Guid? PerformedByUserId { get; set; } // Можно связать с внешней Identity

    [Column("annual_savings",TypeName = "numeric(18,2)")]
    public decimal AnnualSavings { get; set; }

    [Column("displaced_fte", TypeName = "numeric(10,2)")]
    public decimal DisplacedFte { get; set; }

    /// <summary>
    /// Фактически применённый % реинвестиций по итогам правил.
    /// </summary>
    [Column("applied_reinvestment_percent", TypeName = "numeric(5,2)")]
    public decimal AppliedReinvestmentPercent { get; set; }

    /// <summary>
    /// Итоговая сумма, которую обязаны реинвестировать.
    /// </summary>
    [Column("required_reinvestment_amount", TypeName = "numeric(18,2)")]
    public decimal RequiredReinvestmentAmount { get; set; }

    /// <summary>
    /// Скопированная на момент расчёта стоимость внедрения.
    /// </summary>
    [Column("implementation_oneoff_cost", TypeName = "numeric(18,2)")]
    public decimal? ImplementationOneOffCost { get; set; }

    public ICollection<CalculationAllocationSnapshot> Allocations { get; set; }
        = new List<CalculationAllocationSnapshot>();
}