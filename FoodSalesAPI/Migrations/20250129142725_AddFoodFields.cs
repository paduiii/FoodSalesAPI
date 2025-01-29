using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSalesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "FoodSales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSold",
                table: "FoodSales",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                table: "FoodSales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "FoodSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "FoodSales");

            migrationBuilder.DropColumn(
                name: "DateSold",
                table: "FoodSales");

            migrationBuilder.DropColumn(
                name: "FoodName",
                table: "FoodSales");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "FoodSales");
        }
    }
}
