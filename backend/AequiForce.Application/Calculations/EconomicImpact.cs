namespace AequiForce.Application.Calculations;

public sealed record EconomicImpact(
    decimal AnnualSavings,
    decimal DisplacedFte,
    string Currency
);