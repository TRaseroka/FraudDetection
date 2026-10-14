namespace FraudDetection.Application.Exceptions;

public class DuplicateFraudAssessmentException : Exception
{
    public DuplicateFraudAssessmentException(string transactionId)
        : base($"An assessment already exists for transaction {transactionId}.")
    {
    }
}