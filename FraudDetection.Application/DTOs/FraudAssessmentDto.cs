namespace FraudDetection.Application.DTOs;

public class FraudAssessmentDto
{
    public string TransactionId { get; set; } = string.Empty;
    public bool IsFraudulent { get; set; }
    public List<string> TriggeredRules { get; set; } = [];
}