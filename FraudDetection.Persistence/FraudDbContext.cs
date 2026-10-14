using FraudDetection.Domain;
using Microsoft.EntityFrameworkCore;
using FraudDetection.Domain.Entities;
namespace FraudDetection.Persistence;

public class FraudDbContext : DbContext
{
    public FraudDbContext(DbContextOptions<FraudDbContext> options)
        : base(options)
    {
    }

    public DbSet<FraudAssessment> FraudAssessments => Set<FraudAssessment>();
    public DbSet<FraudRuleConfiguration> FraudRuleConfigurations=> Set<FraudRuleConfiguration>();
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
    modelBuilder.Entity<FraudRuleConfiguration>(entity =>
{
    entity.ToTable("FraudRuleConfigurations");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.Code)
        .IsRequired()
        .HasMaxLength(100);

    entity.HasIndex(x => x.Code)
        .IsUnique();

    entity.Property(x => x.Name)
        .IsRequired()
        .HasMaxLength(200);

    entity.Property(x => x.Description)
        .HasMaxLength(1000);

    entity.Property(x => x.IsEnabled)
        .IsRequired();

    entity.Property(x => x.RiskScore)
        .IsRequired();

    entity.Property(x => x.ParametersJson)
        .HasColumnType("jsonb")
        .IsRequired();

    entity.Property(x => x.CreatedAt)
        .IsRequired();

    entity.Property(x => x.UpdatedAt)
        .IsRequired();
});
}
}