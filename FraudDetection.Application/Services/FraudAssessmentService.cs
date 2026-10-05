using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;

namespace FraudDetection.Application.Services;

public class FraudAssessmentService : IFraudAssessmentService
{
    private readonly ITransactionClient _transactionClient;
    private readonly IEnumerable<IFraudRule> _fraudRules;
    private readonly IFraudAssessmentRepository _fraudAssessmentRepository;

    public FraudAssessmentService(
        ITransactionClient transactionClient,
        IEnumerable<IFraudRule> fraudRules,
        IFraudAssessmentRepository fraudAssessmentRepository)
    {
        _transactionClient = transactionClient;
        _fraudRules = fraudRules;
        _fraudAssessmentRepository = fraudAssessmentRepository;
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
        var matchedRules = _fraudRules
       .Where(rule => rule.IsMatch(transaction))
       .ToList();

        var triggeredRules = matchedRules
        .Select(rule => rule.Code)
       .ToList();

        var riskScore = matchedRules
        .Sum(rule => rule.RiskScore);

        var riskLevel = riskScore switch
        {
            >= 100 => "High",
            >= 50 => "Medium",
            _ => "Low"
        };

        var assessment = new FraudAssessment
        {
            TransactionId = transaction.Id.ToString(),
            RiskScore = riskScore,
            RiskLevel = riskLevel,
            IsSuspicious = riskScore >= 50,
            TriggeredRules = string.Join(",", triggeredRules),
            EvaluatedAt = DateTime.UtcNow
        };

        await _fraudAssessmentRepository.AddAsync(
            assessment,
            cancellationToken);

        return new FraudAssessmentDto
        {
            TransactionId = transaction.Id.ToString(),
            IsFraudulent = assessment.IsSuspicious,
            TriggeredRules = triggeredRules
        };
    }
}