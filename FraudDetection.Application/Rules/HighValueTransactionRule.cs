using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Configuration;
using Microsoft.Extensions.Options;
namespace FraudDetection.Application.Rules;

public class HighValueTransactionRule : IFraudRule
{
    public string Code => "HIGH_VALUE_TRANSACTION";
    public int RiskScore => _options.HighValueRiskScore;
    private readonly FraudRuleOptions _options;


    public string Description =>
        $"Transaction amount is greater than or equal to {_options.HighValueThreshold}.";

    
    public HighValueTransactionRule(IOptions<FraudRuleOptions> options)
    {
        _options = options.Value;
    }

   public Task<bool> IsMatchAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(transaction.Amount >= _options.HighValueThreshold);
    }
}

