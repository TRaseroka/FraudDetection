using FraudDetection.Domain;

namespace FraudDetection.Application.Interfaces;

public interface IFraudAssessmentRepository
{
    Task AddAsync(
        FraudAssessment assessment,
        CancellationToken cancellationToken = default);
}