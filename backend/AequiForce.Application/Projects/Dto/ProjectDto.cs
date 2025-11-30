using AequiForce.Domain.Enums;

namespace AequiForce.Application.Projects.Dto;

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? BusinessUnit { get; set; }
    public string Currency { get; set; } = null!;
    public decimal BaselineAnnualCost { get; set; }
    public decimal BaselineFte { get; set; }
    public decimal PostAiAnnualCost { get; set; }
    public decimal PostAiFte { get; set; }
    public decimal? ImplementationOneOffCost { get; set; }
    public decimal? DisplacementRiskPercent { get; set; }
    public ProjectStatus Status { get; set; }
}