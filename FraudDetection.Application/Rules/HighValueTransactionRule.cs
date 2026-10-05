using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
namespace FraudDetection.Application.Rules;

public class HighValueTransactionRule : IFraudRule
{
    public string Code => "HIGH_VALUE_TRANSACTION";
    public int RiskScore => 70;

    public string Description =>
        "Transaction amount is greater than or equal to 10,000.";
   public Task<bool> IsMatchAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(transaction.Amount >= 10_000);
    }
}

