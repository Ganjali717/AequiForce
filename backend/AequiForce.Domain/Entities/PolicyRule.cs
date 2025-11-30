using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AequiForce.Domain.Entities;

[Table("policy_rules")]
public class PolicyRule
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("policy_id")]
    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    /// <summary>
    /// Минимальная годовая экономия (в валюте политики), при которой правило действует.
    /// null = без нижней границы.
    /// </summary>
    [Column("min_annual_savings", TypeName = "numeric(18,2)")]
    public decimal? MinAnnualSavings { get; set; }

    /// <summary>
    /// Максимальная годовая экономия, при которой правило действует.
    /// null = без верхней границы.
    /// </summary>
    [Column("max_annual_savings", TypeName = "numeric(18,2)")]
    public decimal? MaxAnnualSavings { get; set; }

    /// <summary>
    /// Минимальное число FTE, которое считается "под угрозой".
    /// </summary>
    [Column("min_displaced_fte", TypeName = "numeric(10,2)")]
    public decimal? MinDisplacedFte { get; set; }

    [Column("max_displaced_fte", TypeName = "numeric(10,2)")]
    public decimal? MaxDisplacedFte { get; set; }

    /// <summary>
    /// Какой % от экономии должен быть реинвестирован при выполнении условий.
    /// </summary>
    [Range(0, 100)]
    [Column("reinvestment_percent")]
    public decimal ReinvestmentPercent { get; set; }

    /// <summary>
    /// Приоритет: чем меньше, тем раньше проверяется правило.
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 0;
}