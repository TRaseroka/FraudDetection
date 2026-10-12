namespace FraudDetection.Domain;

public class TriggeredRule
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RiskScore { get; set; }
}