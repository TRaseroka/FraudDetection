using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.Rules;

public class TransactionVelocityRule : IFraudRule
{
    private readonly ITransactionClient _transactionClient;

    public TransactionVelocityRule(ITransactionClient transactionClient)
    {
        _transactionClient = transactionClient;
    }

    public string Code => "HIGH_TRANSACTION_VELOCITY";

    public string Description =>
        "Customer has made 3 or more transactions within 10 minutes.";

    public int RiskScore => 60;

    public async Task<bool> IsMatchAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default)
    {
        var customerTransactions =
            await _transactionClient.GetByCustomerIdAsync(
                transaction.CustomerId,
                cancellationToken);

        var windowStart =
            transaction.TransactionDate.AddMinutes(-10);

        var recentTransactionCount =
            customerTransactions.Count(x =>
                x.TransactionDate >= windowStart &&
                x.TransactionDate <= transaction.TransactionDate);

        return recentTransactionCount >= 3;
    }
}