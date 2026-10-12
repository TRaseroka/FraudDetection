namespace FraudDetection.Application.DTOs;

public class FraudAssessmentDto
{
    public string TransactionId { get; set; } = string.Empty;

    public int RiskScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public bool IsSuspicious { get; set; }

    public List<TriggeredRuleDto> TriggeredRules { get; set; } = [];
}