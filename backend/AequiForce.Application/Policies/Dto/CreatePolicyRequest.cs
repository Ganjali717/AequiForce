namespace AequiForce.Application.Policies.Dto;

public class CreatePolicyRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string BaseCurrency { get; set; } = "USD";
    public bool IsDefault { get; set; }
    public decimal DefaultReinvestmentPercent { get; set; }

    public List<PolicyRuleDto> Rules { get; set; } = new();
    public List<PolicyAllocationDto> Allocations { get; set; } = new();
}