using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;

namespace FraudDetection.Application.Services;

public class FraudRuleManagementService
    : IFraudRuleManagementService
{
    private readonly IFraudRuleConfigurationRepository _repository;

    public FraudRuleManagementService(
        IFraudRuleConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<FraudRuleConfiguration>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<FraudRuleConfiguration> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var rule = await _repository.GetByCodeAsync(
            code,
            cancellationToken);

        if (rule == null)
        {
            throw new KeyNotFoundException(
                $"Fraud rule '{code}' was not found.");
        }

        return rule;
    }

    public async Task SetEnabledAsync(
        string code,
        bool isEnabled,
        CancellationToken cancellationToken = default)
    {
        var rule = await GetByCodeAsync(
            code,
            cancellationToken);

        if (rule.IsEnabled == isEnabled)
        {
            return;
        }

        rule.IsEnabled = isEnabled;

        await _repository.UpdateAsync(
            rule,
            cancellationToken);
    }
}