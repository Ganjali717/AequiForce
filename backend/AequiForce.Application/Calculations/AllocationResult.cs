using AequiForce.Domain.Enums;

namespace AequiForce.Application.Calculations;

public sealed record AllocationResult(
    AllocationBucket Bucket,
    decimal BucketPercent,
    decimal Amount
);