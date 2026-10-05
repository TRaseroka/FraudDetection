using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Persistence.Repositories;

public class FraudAssessmentRepository : IFraudAssessmentRepository
{
    private readonly FraudDbContext _dbContext;

    public FraudAssessmentRepository(FraudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        FraudAssessment assessment,
        CancellationToken cancellationToken = default)
    {
        _dbContext.FraudAssessments.Add(assessment);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<FraudAssessment>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FraudAssessments
            .AsNoTracking()
            .OrderByDescending(x => x.EvaluatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<FraudAssessment>> GetByTransactionIdAsync(
    string transactionId,
    CancellationToken cancellationToken = default)
{
    return await _dbContext.FraudAssessments
        .AsNoTracking()
        .Where(x => x.TransactionId == transactionId)
        .OrderByDescending(x => x.EvaluatedAt)
        .ToListAsync(cancellationToken);
}
}