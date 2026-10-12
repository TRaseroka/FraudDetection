using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudDetection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueTransactionIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FraudAssessments_TransactionId",
                table: "FraudAssessments",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FraudAssessments_TransactionId",
                table: "FraudAssessments");
        }
    }
}
