using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLineItem_10_24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Line1",
                table: "CustomerResponses");

            migrationBuilder.DropColumn(
                name: "Address_Line2",
                table: "CustomerResponses");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "CustomerResponses");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "CustomerResponses");

            migrationBuilder.RenameColumn(
                name: "Address_Country",
                table: "CustomerResponses",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "CustomerResponses",
                newName: "City");

            migrationBuilder.AddColumn<int>(
                name: "MineralId",
                table: "LineItems",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "CustomerResponses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "CustomerResponses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_MineralId",
                table: "LineItems",
                column: "MineralId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessionResponses_SessionId",
                table: "CheckoutSessionResponses",
                column: "SessionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Minerals_MineralId",
                table: "LineItems",
                column: "MineralId",
                principalTable: "Minerals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Minerals_MineralId",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_MineralId",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_CheckoutSessionResponses_SessionId",
                table: "CheckoutSessionResponses");

            migrationBuilder.DropColumn(
                name: "MineralId",
                table: "LineItems");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "CustomerResponses",
                newName: "Address_Country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "CustomerResponses",
                newName: "Address_City");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Country",
                table: "CustomerResponses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address_City",
                table: "CustomerResponses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Line1",
                table: "CustomerResponses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Line2",
                table: "CustomerResponses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "CustomerResponses",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "CustomerResponses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
