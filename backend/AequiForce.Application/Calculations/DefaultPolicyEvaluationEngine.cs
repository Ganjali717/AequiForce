using AequiForce.Application.Abstractions.Services;
using AequiForce.Domain.Entities;
using AequiForce.Domain.Enums;

namespace AequiForce.Application.Calculations;

public class DefaultPolicyEvaluationEngine : IPolicyEvaluationEngine
{
    public ReinvestmentDecision Evaluate(Policy policy, EconomicImpact impact)
    {
        if (policy is null) throw new ArgumentNullException(nameof(policy));

        // 1. Выбираем подходящее правило по приоритету
        var applicableRule = policy.Rules
            .OrderBy(r => r.Priority)
            .FirstOrDefault(r =>
                MatchesRange(r.MinAnnualSavings, r.MaxAnnualSavings, impact.AnnualSavings) &&
                MatchesRange(r.MinDisplacedFte, r.MaxDisplacedFte, impact.DisplacedFte));

        decimal reinvestPercent = applicableRule?.ReinvestmentPercent
                                  ?? policy.DefaultReinvestmentPercent;

        // Нормализация
        if (reinvestPercent < 0) reinvestPercent = 0;
        if (reinvestPercent > 100) reinvestPercent = 100;

        // 2. Считаем сумму реинвестиций
        var requiredAmount = impact.AnnualSavings * (reinvestPercent / 100m);
        if (requiredAmount < 0)
            requiredAmount = 0;

        // 3. Раскладываем по "карманам"
        var activeAllocations = policy.Allocations
            .Where(a => a.IsActive)
            .ToList();

        if (activeAllocations.Count == 0)
        {
            // Если нет ни одной аллокации — всё уходит в PureSavings.
            var all = new AllocationResult(
                AllocationBucket.PureSavings,
                100m,
                requiredAmount);

            return new ReinvestmentDecision(reinvestPercent, requiredAmount, new[] { all });
        }

        decimal totalPercent = activeAllocations.Sum(a => a.Percentage);
        if (totalPercent <= 0)
        {
            // Аналогично — fallback
            var all = new AllocationResult(
                AllocationBucket.PureSavings,
                100m,
                requiredAmount);

            return new ReinvestmentDecision(reinvestPercent, requiredAmount, new[] { all });
        }

        var allocationResults = new List<AllocationResult>(activeAllocations.Count);
        foreach (var a in activeAllocations)
        {
            var normalizedPercent = a.Percentage / totalPercent * 100m;
            var amount = requiredAmount * (normalizedPercent / 100m);

            allocationResults.Add(new AllocationResult(
                a.Bucket,
                Math.Round(normalizedPercent, 2),
                Math.Round(amount, 2)
            ));
        }

        return new ReinvestmentDecision(
            reinvestPercent,
            Math.Round(requiredAmount, 2),
            allocationResults);
    }

    private static bool MatchesRange(decimal? min, decimal? max, decimal value)
    {
        if (min.HasValue && value < min.Value) return false;
        if (max.HasValue && value > max.Value) return false;
        return true;
    }
}