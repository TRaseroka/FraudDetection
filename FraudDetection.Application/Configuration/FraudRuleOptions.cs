namespace FraudDetection.Application.Configuration;

public class FraudRuleOptions
{
    public const string SectionName = "FraudRules";

    public decimal HighValueThreshold { get; set; } 

    public decimal LargeCashDepositThreshold { get; set; } 

    public int VelocityTransactionCount { get; set; } 

    public int VelocityWindowMinutes { get; set; }
    public int HighValueRiskScore { get; set; }
    public int LargeCashDepositRiskScore { get; set; }
    public int VelocityRiskScore { get; set; }

    public int MediumRiskThreshold { get; set; }
    public int HighRiskThreshold { get; set; }
    public int SuspiciousRiskThreshold { get; set; }
}