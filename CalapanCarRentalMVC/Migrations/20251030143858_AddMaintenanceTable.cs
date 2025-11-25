using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maintenances",
                columns: table => new
                {
                    MaintenanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    MaintenanceType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateScheduled = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServiceProvider = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenances", x => x.MaintenanceId);
                    table.ForeignKey(
                        name: "FK_Maintenances_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 22, 38, 58, 427, DateTimeKind.Local).AddTicks(6693));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 22, 38, 58, 427, DateTimeKind.Local).AddTicks(6845));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 22, 38, 58, 427, DateTimeKind.Local).AddTicks(6848));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 30, 22, 38, 58, 426, DateTimeKind.Local).AddTicks(9200));

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_CarId",
                table: "Maintenances",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Maintenances");

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
    }
}
