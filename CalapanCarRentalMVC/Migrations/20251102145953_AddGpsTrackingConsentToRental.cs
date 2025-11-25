using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddGpsTrackingConsentToRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GpsConsentDate",
                table: "Rentals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GpsTrackingConsent",
                table: "Rentals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 59, 52, 692, DateTimeKind.Local).AddTicks(213));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 59, 52, 692, DateTimeKind.Local).AddTicks(309));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 59, 52, 692, DateTimeKind.Local).AddTicks(312));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 59, 52, 691, DateTimeKind.Local).AddTicks(3150));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GpsConsentDate",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "GpsTrackingConsent",
                table: "Rentals");

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
    }
}
