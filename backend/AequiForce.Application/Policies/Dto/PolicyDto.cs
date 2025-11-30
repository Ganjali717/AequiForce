namespace AequiForce.Application.Policies.Dto;

public class PolicyDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string BaseCurrency { get; set; } = null!;
    public bool IsDefault { get; set; }
    public decimal DefaultReinvestmentPercent { get; set; }
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }

    public List<PolicyRuleDto> Rules { get; set; } = new();
    public List<PolicyAllocationDto> Allocations { get; set; } = new();
}