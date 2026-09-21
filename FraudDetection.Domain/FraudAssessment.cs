namespace FraudDetection.Domain;

public class FraudAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TransactionId { get; set; } = string.Empty;

    public int RiskScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public bool IsSuspicious { get; set; }

    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}