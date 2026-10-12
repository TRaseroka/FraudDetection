using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudDetection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreTriggeredRulesAsJson : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Delete existing fraud assessments
    migrationBuilder.Sql("""
        DELETE FROM "FraudAssessments";
        """);

    // 2. Remove the old text column default
    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules" DROP DEFAULT;
        """);

    // 3. Convert the column to JSONB
    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules"
        TYPE jsonb
        USING '[]'::jsonb;
        """);

    // 4. Allow null, matching the EF migration
    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules" DROP NOT NULL;
        """);
}
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("""
        DELETE FROM "FraudAssessments";
        """);

    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules" DROP DEFAULT;
        """);

    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules"
        TYPE text
        USING ''::text;
        """);

    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules" SET DEFAULT '';
        """);

    migrationBuilder.Sql("""
        ALTER TABLE "FraudAssessments"
        ALTER COLUMN "TriggeredRules" SET NOT NULL;
        """);
}
}
}

