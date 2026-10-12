using FraudDetection.Application.Configuration;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Rules;
using Microsoft.Extensions.Options;
using Xunit;

namespace FraudDetection.Tests.Rules;

public class HighValueTransactionRuleTests
{
    [Theory]
    [InlineData(5000, false)]
    [InlineData(10000, true)]
    [InlineData(15000, true)]
    public async Task IsMatchAsync_ShouldReturnExpectedResult(
        int amount,
        bool expectedResult)
    {
        // Arrange
        var options = Options.Create(new FraudRuleOptions
        {
            HighValueThreshold = 10000
        });

        var rule = new HighValueTransactionRule(options);

        var transaction = new TransactionDto
        {
            Amount = amount
        };

        // Act
        var result = await rule.IsMatchAsync(transaction);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}