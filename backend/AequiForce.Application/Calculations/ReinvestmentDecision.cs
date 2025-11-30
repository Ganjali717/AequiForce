namespace AequiForce.Application.Calculations;

public sealed record ReinvestmentDecision(
    decimal AppliedReinvestmentPercent,
    decimal RequiredReinvestmentAmount,
    IReadOnlyCollection<AllocationResult> Allocations
);