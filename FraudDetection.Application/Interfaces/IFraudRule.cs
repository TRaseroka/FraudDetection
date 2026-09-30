using FraudDetection.Application.DTOs;

namespace FraudDetection.Application.Interfaces;

public interface IFraudRule
{
    string Code { get; }

    bool IsMatch(TransactionDto transaction);
}