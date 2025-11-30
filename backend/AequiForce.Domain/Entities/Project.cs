using AequiForce.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AequiForce.Domain.Entities;

[Table("projects")]
public class Project
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

    [MaxLength(200)]
    [Column("business_unit")]
    public string? BusinessUnit { get; set; }

    [MaxLength(3)]
    [Column("currency")]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Общие годовые затраты до внедрения AI (ФОТ + накладные).
    /// </summary>
    [Column("baseline_annual_cost", TypeName = "numeric(18,2)")]
    public decimal BaselineAnnualCost { get; set; }

    /// <summary>
    /// Эквивалент FTE до внедрения.
    /// </summary>
    [Column("baseline_fte", TypeName = "numeric(10,2)")]
    public decimal BaselineFte { get; set; }

    [Column("post_ai_annual_cost", TypeName = "numeric(18,2)")]
    public decimal PostAiAnnualCost { get; set; }

    [Column("post_ai_fte", TypeName = "numeric(10,2)")]
    public decimal PostAiFte { get; set; }

    /// <summary>
    /// Разовый CAPEX / стоимость внедрения AI.
    /// </summary>
    [Column("implementation_oneoff_cost", TypeName = "numeric(18,2)")]
    public decimal? ImplementationOneOffCost { get; set; }

    /// <summary>
    /// Оценка риска вытеснения (0..100), может использоваться в будущих версиях.
    /// </summary>
    [Column("displacement_risk_percent", TypeName = "numeric(5,2)")]
    public decimal? DisplacementRiskPercent { get; set; }

    [Column("planned_start_date")]
    public DateTimeOffset? PlannedStartDate { get; set; }

    [Column("planned_end_date")]
    public DateTimeOffset? PlannedEndDate { get; set; }

    [Column("status")]
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

    public ICollection<CalculationSnapshot> Calculations { get; set; } = new List<CalculationSnapshot>();
}