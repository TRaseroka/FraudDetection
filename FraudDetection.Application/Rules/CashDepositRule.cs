using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.Rules;

public class CashDepositRule : IFraudRule
{
    public string Code => "LARGE_CASH_DEPOSIT";

    public string Description =>
        "Cash deposit amount is greater than or equal to 5,000.";

    public int RiskScore => 40;

    public Task<bool> IsMatchAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default)
    {
        var isMatch =
            transaction.PaymentMethod == "CashDeposit"
            && transaction.Amount >= 5_000;

        return Task.FromResult(isMatch);
    }
}