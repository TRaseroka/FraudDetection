using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Configuration;
using Microsoft.Extensions.Options;

namespace FraudDetection.Application.Rules;

public class CashDepositRule : IFraudRule
{
    public string Code => "LARGE_CASH_DEPOSIT";

    public string Description =>
            $"Cash deposit amount is greater than or equal to {_options.LargeCashDepositThreshold}";

    public int RiskScore => _options.LargeCashDepositRiskScore;

    private readonly FraudRuleOptions _options;

    public CashDepositRule(IOptions<FraudRuleOptions> options)
    {
        _options = options.Value;
    }

    public Task<bool> IsMatchAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default)
    {
        var isMatch =
            transaction.PaymentMethod == "CashDeposit"
            && transaction.Amount >= _options.LargeCashDepositThreshold;

        return Task.FromResult(isMatch);
    }
}