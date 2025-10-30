using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalDetailsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Rentals",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GovernmentIdPath",
                table: "Rentals",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Rentals",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 17, 50, 2, 485, DateTimeKind.Local).AddTicks(3055));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 17, 50, 2, 485, DateTimeKind.Local).AddTicks(3250));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 17, 50, 2, 485, DateTimeKind.Local).AddTicks(3272));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 17, 50, 2, 484, DateTimeKind.Local).AddTicks(7118));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "GovernmentIdPath",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Rentals");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 12, 41, 7, 809, DateTimeKind.Local).AddTicks(8400));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 12, 41, 7, 809, DateTimeKind.Local).AddTicks(8494));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 12, 41, 7, 809, DateTimeKind.Local).AddTicks(8497));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 12, 41, 7, 809, DateTimeKind.Local).AddTicks(2824));
        }
    }
}
