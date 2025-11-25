using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentReceiptPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentReceiptPath",
                table: "Rentals",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 46, 48, 345, DateTimeKind.Local).AddTicks(6657));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 46, 48, 345, DateTimeKind.Local).AddTicks(6750));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 46, 48, 345, DateTimeKind.Local).AddTicks(6753));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 46, 48, 345, DateTimeKind.Local).AddTicks(714));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentReceiptPath",
                table: "Rentals");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(8301));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(8393));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(8396));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(2635));
        }
    }
}
