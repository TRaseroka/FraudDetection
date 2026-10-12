using FraudDetection.Application.Configuration;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Rules;
using Microsoft.Extensions.Options;
using Xunit;

namespace FraudDetection.Tests.Rules;

public class LargeCashDepositRuleTests
{
    [Theory]
    [InlineData("CashDeposit", 3000, false)]
    [InlineData("CashDeposit", 5000, true)]
    [InlineData("CashDeposit", 8000, true)]
    [InlineData("Card", 8000, false)]
    [InlineData("EFT", 8000, false)]
    public async Task IsMatchAsync_ShouldReturnExpectedResult(
        string paymentMethod,
        int amount,
        bool expectedResult)
    {
        // Arrange
        var options = Options.Create(new FraudRuleOptions
        {
            LargeCashDepositThreshold = 5000
        });

        var rule = new CashDepositRule(options);

        var transaction = new TransactionDto
        {
            PaymentMethod = paymentMethod,
            Amount = amount
        };

        // Act
        var result = await rule.IsMatchAsync(transaction);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}