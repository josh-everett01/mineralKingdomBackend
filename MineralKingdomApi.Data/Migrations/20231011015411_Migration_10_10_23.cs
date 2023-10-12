using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration_10_10_23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_AuctionStatuses_AuctionStatusId",
                table: "Auctions");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionStatusId",
                table: "Auctions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_AuctionStatuses_AuctionStatusId",
                table: "Auctions",
                column: "AuctionStatusId",
                principalTable: "AuctionStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_AuctionStatuses_AuctionStatusId",
                table: "Auctions");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionStatusId",
                table: "Auctions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_AuctionStatuses_AuctionStatusId",
                table: "Auctions",
                column: "AuctionStatusId",
                principalTable: "AuctionStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
