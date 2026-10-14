using FraudDetection.Application.Interfaces;
using FraudDetection.Domain;
using Microsoft.EntityFrameworkCore;
using FraudDetection.Application.Exceptions;
using Npgsql;

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

    try
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException ex)
    when (ex.InnerException is PostgresException pgEx &&
          pgEx.SqlState == PostgresErrorCodes.UniqueViolation &&
          pgEx.ConstraintName == "IX_FraudAssessments_TransactionId")
    {
    _dbContext.Entry(assessment).State = EntityState.Detached;

    throw new DuplicateFraudAssessmentException(
        assessment.TransactionId);
    }
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