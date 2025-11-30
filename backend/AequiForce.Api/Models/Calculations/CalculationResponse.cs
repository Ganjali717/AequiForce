using AequiForce.Domain.Enums;

namespace AequiForce.Api.Models.Calculations;

public sealed class CalculationResponse
{
    public Guid ProjectId { get; set; }
    public Guid PolicyId { get; set; }
    public decimal AnnualSavings { get; set; }
    public decimal DisplacedFte { get; set; }
    public string Currency { get; set; } = null!;
    public decimal AppliedReinvestmentPercent { get; set; }
    public decimal RequiredReinvestmentAmount { get; set; }

    public List<CalculationBucketResponse> Allocations { get; set; } = new();
}

public sealed class CalculationBucketResponse
{
    public AllocationBucket Bucket { get; set; }
    public decimal BucketPercent { get; set; }
    public decimal Amount { get; set; }
}