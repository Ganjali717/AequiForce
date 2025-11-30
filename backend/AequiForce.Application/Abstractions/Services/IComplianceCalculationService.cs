using AequiForce.Application.Calculations;

namespace AequiForce.Application.Abstractions.Services;

public interface IComplianceCalculationService
{
    /// <summary>
    /// Рассчитать дивиденд по проекту и политике (без сохранения в БД).
    /// Если policyId == null, используется политика по умолчанию организации.
    /// </summary>
    Task<CalculationResult> CalculatePreviewAsync(
        Guid projectId,
        Guid? policyId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполнить расчёт и сохранить снапшот в БД.
    /// Возвращает сохранённый результат.
    /// </summary>
    Task<CalculationResult> CalculateAndPersistAsync(
        Guid projectId,
        Guid? policyId,
        Guid? performedByUserId,
        CancellationToken cancellationToken = default);
}