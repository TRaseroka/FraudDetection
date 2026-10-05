using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudDetection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggeredRulesToFraudAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TriggeredRules",
                table: "FraudAssessments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TriggeredRules",
                table: "FraudAssessments");
        }
    }
}
