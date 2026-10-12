using FraudDetection.Application.Configuration;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Rules;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FraudDetection.Tests.Rules;

public class TransactionVelocityRuleTests
{
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(4, true)]
    public async Task IsMatchAsync_ShouldReturnExpectedResult(
        int transactionCount,
        bool expectedResult)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = DateTime.UtcNow;

        var transactions = Enumerable.Range(0, transactionCount)
            .Select(i => new TransactionDto
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Amount = 1000,
                TransactionDate = transactionDate.AddMinutes(-i)
            })
            .ToList();

        var transactionClientMock = new Mock<ITransactionClient>();

        transactionClientMock
            .Setup(x => x.GetByCustomerIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        var options = Options.Create(new FraudRuleOptions
        {
            VelocityTransactionCount = 3,
            VelocityWindowMinutes = 10
        });

        var rule = new TransactionVelocityRule(
            transactionClientMock.Object,
            options);

        var transaction = transactions[0];

        // Act
        var result = await rule.IsMatchAsync(transaction);

        // Assert
        Assert.Equal(expectedResult, result);


        
    }
    [Theory]
[InlineData(-11, false)]
[InlineData(-10, true)]
public async Task IsMatchAsync_ShouldRespectTimeWindow(
    int minutesOffset,
    bool expectedResult)
{
    // Arrange
    var customerId = Guid.NewGuid();
    var transactionDate = DateTime.UtcNow;

    var transactions = new List<TransactionDto>
    {
        new()
        {
            CustomerId = customerId,
            TransactionDate = transactionDate
        },
        new()
        {
            CustomerId = customerId,
            TransactionDate = transactionDate.AddMinutes(-5)
        },
        new()
        {
            CustomerId = customerId,
            TransactionDate = transactionDate.AddMinutes(minutesOffset)
        }
    };

    var clientMock = new Mock<ITransactionClient>();

    clientMock
        .Setup(x => x.GetByCustomerIdAsync(
            customerId,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(transactions);

    var options = Options.Create(new FraudRuleOptions
    {
        VelocityTransactionCount = 3,
        VelocityWindowMinutes = 10
    });

    var rule = new TransactionVelocityRule(
        clientMock.Object,
        options);

    // Act
    var result = await rule.IsMatchAsync(transactions[0]);

    // Assert
    Assert.Equal(expectedResult, result);
}
}