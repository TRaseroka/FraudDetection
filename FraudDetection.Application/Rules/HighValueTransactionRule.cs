using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
namespace FraudDetection.Application.Rules;

public class HighValueTransactionRule : IFraudRule
{
    public string Code => "HIGH_VALUE_TRANSACTION";

    public bool IsMatch(TransactionDto transaction)
    {
        return transaction.Amount >= 10_000;
    }
}

