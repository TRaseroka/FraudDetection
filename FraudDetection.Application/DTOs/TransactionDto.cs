namespace FraudDetection.Application.DTOs;

public class TransactionDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
}