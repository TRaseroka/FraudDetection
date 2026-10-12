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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<FraudAssessment>(entity =>
    {
        entity.OwnsMany(
            assessment => assessment.TriggeredRules,
            rules =>
            {
                rules.ToJson();
            });

            entity.HasIndex(a => a.TransactionId)
              .IsUnique();
    });
}
}