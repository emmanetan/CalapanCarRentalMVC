using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverLicensePathToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverLicensePath",
                table: "Customers",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 15, 4, 41, 644, DateTimeKind.Local).AddTicks(5282));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 15, 4, 41, 644, DateTimeKind.Local).AddTicks(5917));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 15, 4, 41, 644, DateTimeKind.Local).AddTicks(5923));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 15, 4, 41, 643, DateTimeKind.Local).AddTicks(4710));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLicensePath",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 559, DateTimeKind.Local).AddTicks(2154));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 559, DateTimeKind.Local).AddTicks(2241));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 559, DateTimeKind.Local).AddTicks(2244));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 558, DateTimeKind.Local).AddTicks(6511));
        }
    }
}
