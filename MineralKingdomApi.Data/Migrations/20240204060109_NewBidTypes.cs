﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MineralKingdomApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewBidTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationTime",
                table: "Bids",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BidType",
                table: "Bids",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelayedBid",
                table: "Bids",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumBid",
                table: "Bids",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationTime",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "BidType",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "IsDelayedBid",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "MaximumBid",
                table: "Bids");
        }
    }
}
