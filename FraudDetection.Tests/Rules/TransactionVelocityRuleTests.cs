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
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TransactionDate = transactionDate
        },
        new()
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TransactionDate = transactionDate.AddMinutes(-5)
        },
        new()
        {
            Id = Guid.NewGuid(),
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
[Fact]
public async Task IsMatchAsync_ShouldNotCountDuplicateTransactions()
{
    var customerId = Guid.NewGuid();
    var now = DateTime.UtcNow;

    var current = new TransactionDto
    {
        Id = Guid.NewGuid(),
        CustomerId = customerId,
        TransactionDate = now
    };

    var previous = new TransactionDto
    {
        Id = Guid.NewGuid(),
        CustomerId = customerId,
        TransactionDate = now.AddMinutes(-2)
    };

    var transactions = new List<TransactionDto>
    {
        current,
        previous,
        previous // Duplicate transaction
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

    var result = await rule.IsMatchAsync(current);

    Assert.False(result);
}
[Fact]
public async Task IsMatchAsync_ShouldIgnoreFutureTransactions()
{
    var customerId = Guid.NewGuid();
    var now = DateTime.UtcNow;

    var current = new TransactionDto
    {
        Id = Guid.NewGuid(),
        CustomerId = customerId,
        TransactionDate = now
    };

    var transactions = new List<TransactionDto>
    {
        current,
        new()
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TransactionDate = now.AddMinutes(-2)
        },
        new()
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TransactionDate = now.AddMinutes(2)
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

    var result = await rule.IsMatchAsync(current);

    Assert.False(result);
}
}