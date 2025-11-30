using AequiForce.Application.Policies.Dto;

namespace AequiForce.Application.Abstractions.Services;

public interface IPolicyService
{
    Task<IReadOnlyCollection<PolicyDto>> GetPoliciesAsync(Guid organizationId, CancellationToken ct);
    Task<PolicyDto?> GetPolicyAsync(Guid organizationId, Guid policyId, CancellationToken ct);
    Task<PolicyDto> CreatePolicyAsync(Guid organizationId, CreatePolicyRequest request, CancellationToken ct);
    Task<PolicyDto> UpdatePolicyAsync(Guid organizationId, Guid policyId, UpdatePolicyRequest request, CancellationToken ct);
}