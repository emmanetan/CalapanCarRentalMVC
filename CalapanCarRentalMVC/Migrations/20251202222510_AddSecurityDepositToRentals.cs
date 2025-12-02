using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityDepositToRentals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DepositReturnDate",
                table: "Rentals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositStatus",
                table: "Rentals",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDeposit",
                table: "Rentals",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 6, 25, 9, 549, DateTimeKind.Local).AddTicks(7351));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 6, 25, 9, 549, DateTimeKind.Local).AddTicks(7453));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 6, 25, 9, 549, DateTimeKind.Local).AddTicks(7457));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 6, 25, 9, 549, DateTimeKind.Local).AddTicks(544));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositReturnDate",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "DepositStatus",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "SecurityDeposit",
                table: "Rentals");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 4, 11, 26, 361, DateTimeKind.Local).AddTicks(4510));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 4, 11, 26, 361, DateTimeKind.Local).AddTicks(4608));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 4, 11, 26, 361, DateTimeKind.Local).AddTicks(4612));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 3, 4, 11, 26, 360, DateTimeKind.Local).AddTicks(8416));
        }
    }
}
