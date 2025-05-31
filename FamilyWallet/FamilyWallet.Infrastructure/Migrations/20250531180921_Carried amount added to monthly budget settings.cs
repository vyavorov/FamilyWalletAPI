using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Carriedamountaddedtomonthlybudgetsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CarriedOverAmount",
                table: "MonthlyBudgetSettings",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarriedOverAmount",
                table: "MonthlyBudgetSettings");
        }
    }
}
