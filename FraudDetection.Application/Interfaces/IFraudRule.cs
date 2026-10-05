using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Interfaces;

public interface IFraudRule
{
    string Code { get; }

    int RiskScore { get; }

    string Description { get; }

     Task<bool> IsMatchAsync(
        TransactionDto transaction,
        CancellationToken cancellationToken = default);
}