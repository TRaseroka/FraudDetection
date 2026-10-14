using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Configuration;
namespace FraudDetection.Application.Rules;
using Microsoft.Extensions.Options;

public class TransactionVelocityRule : IFraudRule
{
    private readonly ITransactionClient _transactionClient;
      private readonly FraudRuleOptions _options;
    public TransactionVelocityRule(ITransactionClient transactionClient, IOptions<FraudRuleOptions> options)
    {
        _transactionClient = transactionClient;
        _options = options.Value;
    }

    public string Code => "HIGH_TRANSACTION_VELOCITY";

    public string Description =>
        $"Customer has made {_options.VelocityTransactionCount} or more transactions within {_options.VelocityWindowMinutes} minutes.";

    public int RiskScore => _options.VelocityRiskScore;

    public async Task<bool> IsMatchAsync(
    TransactionDto transaction,
    CancellationToken cancellationToken = default)
{
    var customerTransactions =
        await _transactionClient.GetByCustomerIdAsync(
            transaction.CustomerId,
            cancellationToken);

    var windowStart =
        transaction.TransactionDate.AddMinutes(
            -_options.VelocityWindowMinutes);

    // Count distinct previous transactions within the window.
    var previousTransactionCount = customerTransactions
        .Where(x =>
            x.Id != transaction.Id &&
            x.TransactionDate >= windowStart &&
            x.TransactionDate <= transaction.TransactionDate)
        .Select(x => x.Id)
        .Distinct()
        .Count();

    // Include the transaction currently being assessed.
    var totalTransactionCount = previousTransactionCount + 1;

    return totalTransactionCount >=
           _options.VelocityTransactionCount;
}
}