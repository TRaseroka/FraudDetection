using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.Services;

public class FraudAssessmentService : IFraudAssessmentService
{
    private readonly ITransactionClient _transactionClient;
    private readonly IEnumerable<IFraudRule> _fraudRules;

    public FraudAssessmentService(
        ITransactionClient transactionClient,
        IEnumerable<IFraudRule> fraudRules)
    {
        _transactionClient = transactionClient;
        _fraudRules = fraudRules;
    }

    public async Task<FraudAssessmentDto?> AssessTransactionAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionClient.GetByIdAsync(
            transactionId,
            cancellationToken);

        if (transaction is null)
        {
            return null;
        }

        var triggeredRules = _fraudRules
            .Where(rule => rule.IsMatch(transaction))
            .Select(rule => rule.Code)
            .ToList();

        return new FraudAssessmentDto
        {
            TransactionId = transaction.Id.ToString(),
            IsFraudulent = triggeredRules.Count > 0,
            TriggeredRules = triggeredRules
        };
    }
}