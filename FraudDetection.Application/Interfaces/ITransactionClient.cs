using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Interfaces;

public interface ITransactionClient
{
    Task<TransactionDto?> GetByIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default);
}