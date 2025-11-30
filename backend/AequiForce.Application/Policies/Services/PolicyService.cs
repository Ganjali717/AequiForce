// AequiForce.Application/Policies/Services/PolicyService.cs
using AequiForce.Application.Abstractions.Services;
using AequiForce.Application.Policies.Dto;
using AequiForce.Domain;
using AequiForce.Domain.Entities;
using AequiForce.Infrastructure.Persistence;
using System;
using Microsoft.EntityFrameworkCore;

namespace AequiForce.Application.Policies.Services;

public class PolicyService : IPolicyService
{
    private readonly AppDbContext _db;

    public PolicyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<PolicyDto>> GetPoliciesAsync(Guid organizationId, CancellationToken ct)
    {
        var policies = await _db.Policies
            .Include(p => p.Rules)
            .Include(p => p.Allocations)
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync(ct);

        return policies.Select(MapToDto).ToList();
    }

    public async Task<PolicyDto?> GetPolicyAsync(Guid organizationId, Guid policyId, CancellationToken ct)
    {
        var policy = await _db.Policies
            .Include(p => p.Rules)
            .Include(p => p.Allocations)
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == policyId, ct);

        return policy is null ? null : MapToDto(policy);
    }

    public async Task<PolicyDto> CreatePolicyAsync(Guid organizationId, CreatePolicyRequest request, CancellationToken ct)
    {
        var entity = new Policy
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = request.Name,
            Description = request.Description,
            BaseCurrency = request.BaseCurrency,
            IsDefault = request.IsDefault,
            DefaultReinvestmentPercent = request.DefaultReinvestmentPercent,
            EffectiveFrom = DateTimeOffset.UtcNow
        };

        foreach (var r in request.Rules)
        {
            entity.Rules.Add(new PolicyRule
            {
                Id = Guid.NewGuid(),
                MinAnnualSavings = r.MinAnnualSavings,
                MaxAnnualSavings = r.MaxAnnualSavings,
                MinDisplacedFte = r.MinDisplacedFte,
                MaxDisplacedFte = r.MaxDisplacedFte,
                ReinvestmentPercent = r.ReinvestmentPercent,
                Priority = r.Priority
            });
        }

        foreach (var a in request.Allocations)
        {
            entity.Allocations.Add(new PolicyAllocation
            {
                Id = Guid.NewGuid(),
                Bucket = a.Bucket,
                Percentage = a.Percentage,
                IsActive = a.IsActive
            });
        }

        _db.Policies.Add(entity);
        await _db.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    public async Task<PolicyDto> UpdatePolicyAsync(Guid organizationId, Guid policyId, UpdatePolicyRequest request, CancellationToken ct)
    {
        var policy = await _db.Policies
            .Include(p => p.Rules)
            .Include(p => p.Allocations)
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == policyId, ct);

        if (policy is null)
            throw new InvalidOperationException("Policy not found");

        policy.Name = request.Name;
        policy.Description = request.Description;
        policy.IsDefault = request.IsDefault;
        policy.DefaultReinvestmentPercent = request.DefaultReinvestmentPercent;

        // Примитивно: сносим и создаём заново (MVP)
        _db.PolicyRules.RemoveRange(policy.Rules);
        _db.PolicyAllocations.RemoveRange(policy.Allocations);

        policy.Rules.Clear();
        policy.Allocations.Clear();

        foreach (var r in request.Rules)
        {
            policy.Rules.Add(new PolicyRule
            {
                Id = Guid.NewGuid(),
                MinAnnualSavings = r.MinAnnualSavings,
                MaxAnnualSavings = r.MaxAnnualSavings,
                MinDisplacedFte = r.MinDisplacedFte,
                MaxDisplacedFte = r.MaxDisplacedFte,
                ReinvestmentPercent = r.ReinvestmentPercent,
                Priority = r.Priority
            });
        }

        foreach (var a in request.Allocations)
        {
            policy.Allocations.Add(new PolicyAllocation
            {
                Id = Guid.NewGuid(),
                Bucket = a.Bucket,
                Percentage = a.Percentage,
                IsActive = a.IsActive
            });
        }

        await _db.SaveChangesAsync(ct);

        return MapToDto(policy);
    }

    private static PolicyDto MapToDto(Policy policy)
    {
        return new PolicyDto
        {
            Id = policy.Id,
            OrganizationId = policy.OrganizationId,
            Name = policy.Name,
            Description = policy.Description,
            BaseCurrency = policy.BaseCurrency,
            IsDefault = policy.IsDefault,
            DefaultReinvestmentPercent = policy.DefaultReinvestmentPercent,
            EffectiveFrom = policy.EffectiveFrom,
            EffectiveTo = policy.EffectiveTo,
            Rules = policy.Rules
                .OrderBy(r => r.Priority)
                .Select(r => new PolicyRuleDto
                {
                    Id = r.Id,
                    MinAnnualSavings = r.MinAnnualSavings,
                    MaxAnnualSavings = r.MaxAnnualSavings,
                    MinDisplacedFte = r.MinDisplacedFte,
                    MaxDisplacedFte = r.MaxDisplacedFte,
                    ReinvestmentPercent = r.ReinvestmentPercent,
                    Priority = r.Priority
                }).ToList(),
            Allocations = policy.Allocations
                .Select(a => new PolicyAllocationDto
                {
                    Id = a.Id,
                    Bucket = a.Bucket,
                    Percentage = a.Percentage,
                    IsActive = a.IsActive
                }).ToList()
        };
    }
}
