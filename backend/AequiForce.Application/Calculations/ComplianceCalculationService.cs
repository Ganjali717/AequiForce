using AequiForce.Application.Abstractions.Services;
using AequiForce.Application.Calculations;
using AequiForce.Domain.Entities;
using AequiForce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AequiForce.Infrastructure.Services;

public class ComplianceCalculationService : IComplianceCalculationService
{
    private readonly AppDbContext _db;
    private readonly IPolicyEvaluationEngine _engine;

    public ComplianceCalculationService(
        AppDbContext db,
        IPolicyEvaluationEngine engine)
    {
        _db = db;
        _engine = engine;
    }

    public async Task<CalculationResult> CalculatePreviewAsync(
        Guid projectId,
        Guid? policyId = null,
        CancellationToken cancellationToken = default)
    {
        var project = await _db.Projects
            .Include(p => p.Organization)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

        if (project == null)
            throw new InvalidOperationException($"Project {projectId} not found.");

        var policy = await ResolvePolicyAsync(project.OrganizationId, policyId, cancellationToken);
        if (policy == null)
            throw new InvalidOperationException($"Policy not found for organization {project.OrganizationId}.");

        var impact = BuildImpact(project);
        var decision = _engine.Evaluate(policy, impact);

        return MapToResult(project, policy, impact, decision);
    }

    public async Task<CalculationResult> CalculateAndPersistAsync(
        Guid projectId,
        Guid? policyId,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var project = await _db.Projects
            .Include(p => p.Organization)
            .Include(p => p.Calculations)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

        if (project == null)
            throw new InvalidOperationException($"Project {projectId} not found.");

        var policy = await ResolvePolicyAsync(project.OrganizationId, policyId, cancellationToken);
        if (policy == null)
            throw new InvalidOperationException($"Policy not found for organization {project.OrganizationId}.");

        var impact = BuildImpact(project);
        var decision = _engine.Evaluate(policy, impact);

        // Сохраняем снапшот
        var snapshot = new CalculationSnapshot
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            PolicyId = policy.Id,
            CalculatedAt = DateTimeOffset.UtcNow,
            PerformedByUserId = performedByUserId,
            AnnualSavings = impact.AnnualSavings,
            DisplacedFte = impact.DisplacedFte,
            AppliedReinvestmentPercent = decision.AppliedReinvestmentPercent,
            RequiredReinvestmentAmount = decision.RequiredReinvestmentAmount,
            ImplementationOneOffCost = project.ImplementationOneOffCost
        };

        foreach (var alloc in decision.Allocations)
        {
            snapshot.Allocations.Add(new CalculationAllocationSnapshot
            {
                Id = Guid.NewGuid(),
                CalculationSnapshotId = snapshot.Id,
                Bucket = alloc.Bucket,
                BucketPercent = alloc.BucketPercent,
                Amount = alloc.Amount
            });
        }

        _db.CalculationSnapshots.Add(snapshot);
        await _db.SaveChangesAsync(cancellationToken);

        return MapToResult(project, policy, impact, decision);
    }

    private static EconomicImpact BuildImpact(Project project)
    {
        var annualSavings = project.BaselineAnnualCost - project.PostAiAnnualCost;
        var displacedFte = project.BaselineFte - project.PostAiFte;

        if (displacedFte < 0) displacedFte = 0;

        return new EconomicImpact(
            AnnualSavings: annualSavings,
            DisplacedFte: displacedFte,
            Currency: project.Currency
        );
    }

    private async Task<Policy?> ResolvePolicyAsync(
        Guid organizationId,
        Guid? policyId,
        CancellationToken cancellationToken)
    {
        if (policyId.HasValue)
        {
            return await _db.Policies
                .Include(p => p.Rules)
                .Include(p => p.Allocations)
                .FirstOrDefaultAsync(p =>
                        p.Id == policyId.Value &&
                        p.OrganizationId == organizationId,
                    cancellationToken);
        }

        // Берём политику по умолчанию
        var defaultPolicy = await _db.Policies
            .Include(p => p.Rules)
            .Include(p => p.Allocations)
            .Where(p => p.OrganizationId == organizationId && p.IsDefault)
            .OrderByDescending(p => p.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (defaultPolicy != null)
            return defaultPolicy;

        // Fallback: любую последнюю политику организации
        return await _db.Policies
            .Include(p => p.Rules)
            .Include(p => p.Allocations)
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static CalculationResult MapToResult(
        Project project,
        Policy policy,
        EconomicImpact impact,
        ReinvestmentDecision decision)
    {
        return new CalculationResult(
            ProjectId: project.Id,
            PolicyId: policy.Id,
            Impact: impact,
            AppliedReinvestmentPercent: decision.AppliedReinvestmentPercent,
            RequiredReinvestmentAmount: decision.RequiredReinvestmentAmount,
            Allocations: decision.Allocations.ToArray()
        );
    }
}