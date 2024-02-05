using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionConcurrencyControlToBid2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Bids",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Bids");
        }
    }
}
