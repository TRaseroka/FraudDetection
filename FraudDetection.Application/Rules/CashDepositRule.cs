using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;

namespace FraudDetection.Application.Rules;

public class CashDepositRule : IFraudRule
{
    public string Code => "CASH_DEPOSIT";

    public bool IsMatch(TransactionDto transaction)
    {
        return transaction.PaymentMethod == "CashDeposit" && transaction.Amount >= 5_000;
    }
}