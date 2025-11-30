namespace AequiForce.Application.Calculations;

public sealed record CalculationResult(
    Guid ProjectId,
    Guid PolicyId,
    EconomicImpact Impact,
    decimal AppliedReinvestmentPercent,
    decimal RequiredReinvestmentAmount,
    IReadOnlyCollection<AllocationResult> Allocations
);