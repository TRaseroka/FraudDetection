
using AutoMapper;
using FraudDetection.Application.Configuration;
using FraudDetection.Application.DTOs;
using FraudDetection.Application.Interfaces;
using FraudDetection.Application.Mappings;
using FraudDetection.Application.Services;
using FraudDetection.Domain;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using FraudDetection.Application.Exceptions;

namespace FraudDetection.Tests.Services;

public class FraudAssessmentServiceTests
{
    private readonly IMapper _mapper;

    public FraudAssessmentServiceTests()
    {
        var mapperConfiguration = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile<FraudMappingProfile>();
        },
        NullLoggerFactory.Instance);

    mapperConfiguration.AssertConfigurationIsValid();

    _mapper = mapperConfiguration.CreateMapper();
}
    

    private static IOptions<FraudRuleOptions> CreateOptions()
    {
        return Options.Create(new FraudRuleOptions
        {
            MediumRiskThreshold = 50,
            HighRiskThreshold = 100,
            SuspiciousRiskThreshold = 50
        });
    }

    [Fact]
    public async Task AssessAsync_WhenTwoRulesMatch_ShouldCalculateHighRisk()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();

        var transaction = new TransactionDto
        {
            Id = Guid.Parse(transactionId),
            CustomerId = Guid.NewGuid(),
            Amount = 15000,
            PaymentMethod = "CashDeposit",
            TransactionDate = DateTime.UtcNow
        };

        var transactionClient = new Mock<ITransactionClient>();

        transactionClient
            .Setup(x => x.GetByIdAsync(
                transactionId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var highValueRule = new Mock<IFraudRule>();

        highValueRule.SetupGet(x => x.Code)
            .Returns("HIGH_VALUE_TRANSACTION");

        highValueRule.SetupGet(x => x.Description)
            .Returns("High value transaction");

        highValueRule.SetupGet(x => x.RiskScore)
            .Returns(70);

        highValueRule
            .Setup(x => x.IsMatchAsync(
                It.IsAny<TransactionDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var cashDepositRule = new Mock<IFraudRule>();

        cashDepositRule.SetupGet(x => x.Code)
            .Returns("LARGE_CASH_DEPOSIT");

        cashDepositRule.SetupGet(x => x.Description)
            .Returns("Large cash deposit");

        cashDepositRule.SetupGet(x => x.RiskScore)
            .Returns(40);

        cashDepositRule
            .Setup(x => x.IsMatchAsync(
                It.IsAny<TransactionDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var repository = new Mock<IFraudAssessmentRepository>();

        var rules = new List<IFraudRule>
        {
            highValueRule.Object,
            cashDepositRule.Object
        };

        var service = new FraudAssessmentService(
            transactionClient.Object,
            rules,
            repository.Object,
            CreateOptions(),
            _mapper);

        // Act
        var result = await service.AssessTransactionAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(110, result.RiskScore);
        Assert.Equal("High", result.RiskLevel);
        Assert.True(result.IsSuspicious);
        Assert.Equal(2, result.TriggeredRules.Count);

        // Verify AutoMapper mapped triggered rules correctly
        Assert.Contains(result.TriggeredRules, rule =>
            rule.Code == "HIGH_VALUE_TRANSACTION" &&
            rule.Description == "High value transaction" &&
            rule.RiskScore == 70);

        Assert.Contains(result.TriggeredRules, rule =>
            rule.Code == "LARGE_CASH_DEPOSIT" &&
            rule.Description == "Large cash deposit" &&
            rule.RiskScore == 40);

        // Verify the entity was persisted
        repository.Verify(x => x.AddAsync(
            It.Is<FraudAssessment>(assessment =>
                assessment.TransactionId == transactionId &&
                assessment.RiskScore == 110 &&
                assessment.RiskLevel == "High" &&
                assessment.IsSuspicious &&
                assessment.TriggeredRules.Count == 2),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AssessAsync_WhenNoRulesMatch_ShouldReturnLowRisk()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();

        var transaction = new TransactionDto
        {
            Id = Guid.Parse(transactionId),
            CustomerId = Guid.NewGuid(),
            Amount = 100,
            PaymentMethod = "Card",
            TransactionDate = DateTime.UtcNow
        };

        var transactionClient = new Mock<ITransactionClient>();

        transactionClient
            .Setup(x => x.GetByIdAsync(
                transactionId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var rule = new Mock<IFraudRule>();

        rule
            .Setup(x => x.IsMatchAsync(
                It.IsAny<TransactionDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var repository = new Mock<IFraudAssessmentRepository>();
        repository
         .Setup(x => x.GetByTransactionIdAsync(
        transactionId,
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(Array.Empty<FraudAssessment>());

        var service = new FraudAssessmentService(
            transactionClient.Object,
            new List<IFraudRule> { rule.Object },
            repository.Object,
            CreateOptions(),
            _mapper);

        // Act
        var result = await service.AssessTransactionAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(0, result.RiskScore);
        Assert.Equal("Low", result.RiskLevel);
        Assert.False(result.IsSuspicious);
        Assert.Empty(result.TriggeredRules);

        repository.Verify(x => x.AddAsync(
            It.Is<FraudAssessment>(assessment =>
                assessment.RiskScore == 0 &&
                assessment.RiskLevel == "Low" &&
                !assessment.IsSuspicious &&
                assessment.TriggeredRules.Count == 0),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AssessAsync_WhenTransactionNotFound_ShouldThrowException()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();

        var transactionClient = new Mock<ITransactionClient>();

        transactionClient
            .Setup(x => x.GetByIdAsync(
                transactionId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionDto?)null);

        var repository = new Mock<IFraudAssessmentRepository>();

        repository
    .Setup(x => x.GetByTransactionIdAsync(
        transactionId,
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(Array.Empty<FraudAssessment>());
        var service = new FraudAssessmentService(
            transactionClient.Object,
            new List<IFraudRule>(),
            repository.Object,
            CreateOptions(),
            _mapper);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.AssessTransactionAsync(transactionId));

        Assert.Contains(transactionId, exception.Message);

        repository.Verify(x => x.AddAsync(
            It.IsAny<FraudAssessment>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AssessAsync_WhenAssessmentExists_ShouldReturnExistingAssessment()
    {
        // Arrange
        var transactionId = Guid.NewGuid().ToString();

        var existingAssessment = new FraudAssessment
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            RiskScore = 70,
            RiskLevel = "Medium",
            IsSuspicious = true,
            TriggeredRules = new List<TriggeredRule>
            {
                new TriggeredRule
                {
                    Code = "HIGH_VALUE_TRANSACTION",
                    Description = "High value transaction",
                    RiskScore = 70
                }
            },
            EvaluatedAt = DateTime.UtcNow
        };

        var repository = new Mock<IFraudAssessmentRepository>();

        repository
          .Setup(x => x.GetByTransactionIdAsync(
        transactionId,
        It.IsAny<CancellationToken>()))
        .ReturnsAsync(
        new List<FraudAssessment>
        {
            existingAssessment
        });

        var transactionClient = new Mock<ITransactionClient>();

        var rule = new Mock<IFraudRule>();

        var service = new FraudAssessmentService(
            transactionClient.Object,
            new List<IFraudRule> { rule.Object },
            repository.Object,
            CreateOptions(),
            _mapper);

        // Act
        var result = await service.AssessTransactionAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(70, result.RiskScore);
        Assert.Equal("Medium", result.RiskLevel);
        Assert.True(result.IsSuspicious);

        Assert.Single(result.TriggeredRules);

        Assert.Equal(
            "HIGH_VALUE_TRANSACTION",
            result.TriggeredRules[0].Code);

        Assert.Equal(
            "High value transaction",
            result.TriggeredRules[0].Description);

        Assert.Equal(
            70,
            result.TriggeredRules[0].RiskScore);

        // Transaction API must not be called again
        transactionClient.Verify(x => x.GetByIdAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()),
            Times.Never);

        // Fraud rules must not be evaluated again
        rule.Verify(x => x.IsMatchAsync(
            It.IsAny<TransactionDto>(),
            It.IsAny<CancellationToken>()),
            Times.Never);

        // No duplicate assessment should be saved
        repository.Verify(x => x.AddAsync(
            It.IsAny<FraudAssessment>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
public async Task AssessAsync_WhenConcurrentInsertOccurs_ShouldReturnExistingAssessment()
{
    // Arrange
    var transactionId = Guid.NewGuid().ToString();

    var transaction = new TransactionDto
    {
        Id = Guid.Parse(transactionId),
        CustomerId = Guid.NewGuid(),
        Amount = 15000,
        PaymentMethod = "Card",
        TransactionDate = DateTime.UtcNow
    };

    var existingAssessment = new FraudAssessment
    {
        Id = Guid.NewGuid(),
        TransactionId = transactionId,
        RiskScore = 70,
        RiskLevel = "Medium",
        IsSuspicious = true,
        EvaluatedAt = DateTime.UtcNow,
        TriggeredRules = new List<TriggeredRule>
        {
            new TriggeredRule
            {
                Code = "HIGH_VALUE_TRANSACTION",
                Description = "High value transaction",
                RiskScore = 70
            }
        }
    };

    var transactionClient = new Mock<ITransactionClient>();

    transactionClient
        .Setup(x => x.GetByIdAsync(
            transactionId,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(transaction);

    var repository = new Mock<IFraudAssessmentRepository>();

    // First lookup: no assessment exists.
    // Second lookup: another request has saved it.
    repository
        .SetupSequence(x => x.GetByTransactionIdAsync(
            transactionId,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(Array.Empty<FraudAssessment>())
        .ReturnsAsync(new List<FraudAssessment>
        {
            existingAssessment
        });

    repository
        .Setup(x => x.AddAsync(
            It.IsAny<FraudAssessment>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(
            new DuplicateFraudAssessmentException(transactionId));

    var rule = new Mock<IFraudRule>();

    rule.SetupGet(x => x.Code)
        .Returns("HIGH_VALUE_TRANSACTION");

    rule.SetupGet(x => x.Description)
        .Returns("High value transaction");

    rule.SetupGet(x => x.RiskScore)
        .Returns(70);

    rule
        .Setup(x => x.IsMatchAsync(
            It.IsAny<TransactionDto>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    var service = new FraudAssessmentService(
        transactionClient.Object,
        new List<IFraudRule> { rule.Object },
        repository.Object,
        CreateOptions(),
        _mapper);

    // Act
    var result = await service.AssessTransactionAsync(transactionId);

    // Assert
    Assert.NotNull(result);
 //   Assert.Equal(existingAssessment.Id, result.Id);
    Assert.Equal(transactionId, result.TransactionId);
    Assert.Equal(70, result.RiskScore);
    Assert.Equal("Medium", result.RiskLevel);
    Assert.True(result.IsSuspicious);

    Assert.Single(result.TriggeredRules);

    repository.Verify(x => x.AddAsync(
        It.IsAny<FraudAssessment>(),
        It.IsAny<CancellationToken>()),
        Times.Once);

    repository.Verify(x => x.GetByTransactionIdAsync(
        transactionId,
        It.IsAny<CancellationToken>()),
        Times.Exactly(2));
}
}
