using FraudDetection.Domain;
using FraudDetection.Application.Interfaces;
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
}