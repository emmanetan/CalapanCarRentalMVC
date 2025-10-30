using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddGasTypeToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GasType",
                table: "Cars",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "GasType" },
                values: new object[] { new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(8301), "Gasoline" });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "GasType" },
                values: new object[] { new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(8393), "Gasoline" });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "GasType" },
                values: new object[] { new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(8396), "Diesel" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 18, 16, 50, 913, DateTimeKind.Local).AddTicks(2635));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GasType",
                table: "Cars");

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
    }
}
