using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateMineralModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "ImageURLs",
                table: "Minerals",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Minerals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VideoURL",
                table: "Minerals",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURLs",
                table: "Minerals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Minerals");

            migrationBuilder.DropColumn(
                name: "VideoURL",
                table: "Minerals");
        }
    }
}
