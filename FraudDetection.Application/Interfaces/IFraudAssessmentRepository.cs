using FraudDetection.Domain;

namespace FraudDetection.Application.Interfaces;

public interface IFraudAssessmentRepository
{
    Task AddAsync(
        FraudAssessment assessment,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FraudAssessment>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
      Task<IReadOnlyCollection<FraudAssessment>> GetByTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default);
}