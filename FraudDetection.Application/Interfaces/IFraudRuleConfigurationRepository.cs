using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Interfaces;

public interface IFraudRuleConfigurationRepository
{
    Task<IReadOnlyCollection<FraudRuleConfiguration>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FraudRuleConfiguration>> GetEnabledAsync(
        CancellationToken cancellationToken = default);

    Task<FraudRuleConfiguration?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        FraudRuleConfiguration configuration,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        FraudRuleConfiguration configuration,
        CancellationToken cancellationToken = default);
}