using AequiForce.Application.Abstractions.Services;
using AequiForce.Application.Policies.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AequiForce.Api.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:guid}/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PoliciesController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PolicyDto>>> GetPolicies(
        Guid organizationId,
        CancellationToken ct)
    {
        var policies = await _policyService.GetPoliciesAsync(organizationId, ct);
        return Ok(policies);
    }

    [HttpGet("{policyId:guid}")]
    public async Task<ActionResult<PolicyDto>> GetPolicy(
        Guid organizationId,
        Guid policyId,
        CancellationToken ct)
    {
        var policy = await _policyService.GetPolicyAsync(organizationId, policyId, ct);
        if (policy is null)
            return NotFound();

        return Ok(policy);
    }

    [HttpPost]
    public async Task<ActionResult<PolicyDto>> CreatePolicy(
        Guid organizationId,
        [FromBody] CreatePolicyRequest request,
        CancellationToken ct)
    {
        var result = await _policyService.CreatePolicyAsync(organizationId, request, ct);
        return CreatedAtAction(nameof(GetPolicy),
            new { organizationId, policyId = result.Id, version = "1.0" }, result);
    }

    [HttpPut("{policyId:guid}")]
    public async Task<ActionResult<PolicyDto>> UpdatePolicy(
        Guid organizationId,
        Guid policyId,
        [FromBody] UpdatePolicyRequest request,
        CancellationToken ct)
    {
        var result = await _policyService.UpdatePolicyAsync(organizationId, policyId, request, ct);
        return Ok(result);
    }
}
