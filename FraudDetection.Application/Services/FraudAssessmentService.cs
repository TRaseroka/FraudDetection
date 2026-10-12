using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;
using FraudDetection.Application.Configuration;
using Microsoft.Extensions.Options;
using AutoMapper;

namespace FraudDetection.Application.Services;

public class FraudAssessmentService : IFraudAssessmentService
{
    private readonly ITransactionClient _transactionClient;
    private readonly IEnumerable<IFraudRule> _fraudRules;
    private readonly IFraudAssessmentRepository _fraudAssessmentRepository;
    private readonly FraudRuleOptions _options;
    private readonly IMapper _mapper;
    public FraudAssessmentService(
        ITransactionClient transactionClient,
        IEnumerable<IFraudRule> fraudRules,
        IFraudAssessmentRepository fraudAssessmentRepository,
        IOptions<FraudRuleOptions> options,
        IMapper mapper )
    {
        _transactionClient = transactionClient;
        _fraudRules = fraudRules;
        _fraudAssessmentRepository = fraudAssessmentRepository;
        _options = options.Value;
        _mapper = mapper;}

    public async Task<FraudAssessmentDto?> AssessTransactionAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
     var existingAssessments =
            await _fraudAssessmentRepository.GetByTransactionIdAsync(
                transactionId,
                cancellationToken);
         var existingAssessment = existingAssessments?.FirstOrDefault();
        if (existingAssessment != null)
        {
            return _mapper.Map<FraudAssessmentDto>(existingAssessment);
        }

        var transaction = await _transactionClient.GetByIdAsync(
            transactionId,
            cancellationToken);

        if (transaction == null)
        {
        throw new KeyNotFoundException(
        $"Transaction {transactionId} was not found.");
        }

        var triggeredRules = new List<TriggeredRule>();

        foreach (var rule in _fraudRules)
        {
            if (await rule.IsMatchAsync(transaction, cancellationToken))
            {
                triggeredRules.Add(new TriggeredRule
                {
                    Code = rule.Code,
                    Description = rule.Description,
                    RiskScore = rule.RiskScore
                });
            }
        }

        var riskScore = triggeredRules.Sum(rule => rule.RiskScore);

        var riskLevel =
            riskScore >= _options.HighRiskThreshold ? "High" :
            riskScore >= _options.MediumRiskThreshold ? "Medium" :
            "Low";

        var isSuspicious =
            riskScore >= _options.SuspiciousRiskThreshold;

        var assessment = new FraudAssessment
        {
            TransactionId = transaction.Id.ToString(),
            RiskScore = riskScore,
            RiskLevel = riskLevel,
            IsSuspicious = isSuspicious,
            TriggeredRules = triggeredRules,
            EvaluatedAt = DateTime.UtcNow
        };

        await _fraudAssessmentRepository.AddAsync(
            assessment,
            cancellationToken);

      return _mapper.Map<FraudAssessmentDto>(assessment);
    }
}