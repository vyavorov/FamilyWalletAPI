using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsGlobalfieldaddedtocategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                table: "Categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGlobal",
                table: "Categories");
        }
    }
}
