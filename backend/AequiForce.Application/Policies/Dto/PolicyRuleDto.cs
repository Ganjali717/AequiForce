namespace AequiForce.Application.Policies.Dto;

public class PolicyRuleDto
{
    public Guid Id { get; set; }
    public decimal? MinAnnualSavings { get; set; }
    public decimal? MaxAnnualSavings { get; set; }
    public decimal? MinDisplacedFte { get; set; }
    public decimal? MaxDisplacedFte { get; set; }
    public decimal ReinvestmentPercent { get; set; }
    public int Priority { get; set; }
}