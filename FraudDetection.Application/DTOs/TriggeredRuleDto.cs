namespace FraudDetection.Application.DTOs;

public class TriggeredRuleDto
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RiskScore { get; set; }
}