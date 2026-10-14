using FraudDetection.Application.Interfaces;
using FraudDetection.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Persistence.Repositories;

public class FraudRuleConfigurationRepository
    : IFraudRuleConfigurationRepository
{
    private readonly FraudDbContext _dbContext;

    public FraudRuleConfigurationRepository(FraudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<FraudRuleConfiguration>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FraudRuleConfigurations
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<FraudRuleConfiguration>> GetEnabledAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FraudRuleConfigurations
            .AsNoTracking()
            .Where(x => x.IsEnabled)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<FraudRuleConfiguration?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FraudRuleConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddAsync(
        FraudRuleConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.FraudRuleConfigurations.AddAsync(
            configuration,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        FraudRuleConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        configuration.UpdatedAt = DateTime.UtcNow;

        _dbContext.FraudRuleConfigurations.Update(configuration);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}