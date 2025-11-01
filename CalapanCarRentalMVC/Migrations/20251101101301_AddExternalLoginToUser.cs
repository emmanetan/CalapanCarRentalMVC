using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalLoginToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProvider",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProviderId",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 559, DateTimeKind.Local).AddTicks(2154));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 559, DateTimeKind.Local).AddTicks(2241));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 18, 13, 0, 559, DateTimeKind.Local).AddTicks(2244));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ExternalLoginProvider", "ExternalLoginProviderId" },
                values: new object[] { new DateTime(2025, 11, 1, 18, 13, 0, 558, DateTimeKind.Local).AddTicks(6511), null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLoginProvider",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalLoginProviderId",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 18, 7, 40, 172, DateTimeKind.Local).AddTicks(7157));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 18, 7, 40, 172, DateTimeKind.Local).AddTicks(7311));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 18, 7, 40, 172, DateTimeKind.Local).AddTicks(7314));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 18, 7, 40, 172, DateTimeKind.Local).AddTicks(58));
        }
    }
}
