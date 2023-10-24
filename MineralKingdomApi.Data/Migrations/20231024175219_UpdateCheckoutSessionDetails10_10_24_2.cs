using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckoutSessionDetails10_10_24_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_CheckoutSessionDetails_CheckoutSessionDetailsId",
                table: "LineItems");

            migrationBuilder.AlterColumn<int>(
                name: "CheckoutSessionDetailsId",
                table: "LineItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_CheckoutSessionDetails_CheckoutSessionDetailsId",
                table: "LineItems",
                column: "CheckoutSessionDetailsId",
                principalTable: "CheckoutSessionDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_CheckoutSessionDetails_CheckoutSessionDetailsId",
                table: "LineItems");

            migrationBuilder.AlterColumn<int>(
                name: "CheckoutSessionDetailsId",
                table: "LineItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_CheckoutSessionDetails_CheckoutSessionDetailsId",
                table: "LineItems",
                column: "CheckoutSessionDetailsId",
                principalTable: "CheckoutSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
