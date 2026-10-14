using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces;

public interface IFraudRuleManagementService
{
    Task<IReadOnlyCollection<FraudRuleConfiguration>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FraudRuleConfiguration> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task SetEnabledAsync(
        string code,
        bool isEnabled,
        CancellationToken cancellationToken = default);
}

