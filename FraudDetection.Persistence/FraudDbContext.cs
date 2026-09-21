using FraudDetection.Domain;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.Persistence;

public class FraudDbContext : DbContext
{
    public FraudDbContext(DbContextOptions<FraudDbContext> options)
        : base(options)
    {
    }

    public DbSet<FraudAssessment> FraudAssessments => Set<FraudAssessment>();
}