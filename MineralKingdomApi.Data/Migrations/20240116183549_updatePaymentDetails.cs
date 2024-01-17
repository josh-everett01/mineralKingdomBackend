﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatePaymentDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "PaymentDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaymentDetails");
        }
    }
}
