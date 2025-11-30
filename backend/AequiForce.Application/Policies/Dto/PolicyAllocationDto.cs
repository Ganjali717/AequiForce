using AequiForce.Domain.Enums;

namespace AequiForce.Application.Policies.Dto;

public class PolicyAllocationDto
{
    public Guid Id { get; set; }
    public AllocationBucket Bucket { get; set; }
    public decimal Percentage { get; set; }
    public bool IsActive { get; set; }
}