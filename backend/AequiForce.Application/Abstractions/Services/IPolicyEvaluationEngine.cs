using AequiForce.Application.Calculations;
using AequiForce.Domain.Entities;

namespace AequiForce.Application.Abstractions.Services;

public interface IPolicyEvaluationEngine
{
    /// <summary>
    /// На основе политики и экономического эффекта возвращает решение:
    /// какой % применить и как разложить по карманам.
    /// </summary>
    ReinvestmentDecision Evaluate(Policy policy, EconomicImpact impact);
}