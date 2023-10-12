using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Minerals_MineralId",
                table: "Auctions");

            migrationBuilder.AlterColumn<int>(
                name: "MineralId",
                table: "Auctions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Minerals_MineralId",
                table: "Auctions",
                column: "MineralId",
                principalTable: "Minerals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Minerals_MineralId",
                table: "Auctions");

            migrationBuilder.AlterColumn<int>(
                name: "MineralId",
                table: "Auctions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Minerals_MineralId",
                table: "Auctions",
                column: "MineralId",
                principalTable: "Minerals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
