using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Balanceaddedtouserandusergroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Users",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "FamilyGroups",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "FamilyGroups");
        }
    }
}
