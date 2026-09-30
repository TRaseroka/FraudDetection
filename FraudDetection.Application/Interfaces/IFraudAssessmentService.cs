using FraudDetection.Application.DTOs;

public interface IFraudAssessmentService
{
    Task<FraudAssessmentDto?> AssessTransactionAsync(
        string transactionId,
        CancellationToken cancellationToken = default);
}