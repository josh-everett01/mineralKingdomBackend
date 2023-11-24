using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsWinnerNotifiedBoolPropl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWinnerNotified",
                table: "Auctions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWinnerNotified",
                table: "Auctions");
        }
    }
}
