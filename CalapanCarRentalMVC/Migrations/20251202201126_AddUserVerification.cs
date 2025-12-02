using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalapanCarRentalMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Cars_CarId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Cars_CarId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "CarId",
                table: "Rentals",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Rentals_CarId",
                table: "Rentals",
                newName: "IX_Rentals_VehicleId");

            migrationBuilder.RenameColumn(
                name: "CarId",
                table: "Maintenances",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_CarId",
                table: "Maintenances",
                newName: "IX_Maintenances_VehicleId");

            migrationBuilder.RenameColumn(
                name: "CarId",
                table: "Cars",
                newName: "VehicleId");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByAdmin",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LocationTrackingEnabled",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LocationTrackingEnabledDate",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "is_Admin",
                table: "Users",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "LocationHistories",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LicenseCode",
                table: "Customers",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Coding",
                table: "Cars",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 1,
                columns: new[] { "Coding", "CreatedAt" },
                values: new object[] { "Monday", new DateTime(2025, 12, 3, 4, 11, 26, 361, DateTimeKind.Local).AddTicks(4510) });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 2,
                columns: new[] { "Coding", "CreatedAt" },
                values: new object[] { "Thursday", new DateTime(2025, 12, 3, 4, 11, 26, 361, DateTimeKind.Local).AddTicks(4608) });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "VehicleId",
                keyValue: 3,
                columns: new[] { "Coding", "CreatedAt" },
                values: new object[] { "Monday", new DateTime(2025, 12, 3, 4, 11, 26, 361, DateTimeKind.Local).AddTicks(4612) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsVerifiedByAdmin", "LocationTrackingEnabled", "LocationTrackingEnabledDate", "VerifiedDate", "is_Admin" },
                values: new object[] { new DateTime(2025, 12, 3, 4, 11, 26, 360, DateTimeKind.Local).AddTicks(8416), false, false, null, null, (byte)0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Cars_VehicleId",
                table: "Maintenances",
                column: "VehicleId",
                principalTable: "Cars",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Cars_VehicleId",
                table: "Rentals",
                column: "VehicleId",
                principalTable: "Cars",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Cars_VehicleId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Cars_VehicleId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationTrackingEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationTrackingEnabledDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "is_Admin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "LocationHistories");

            migrationBuilder.DropColumn(
                name: "LicenseCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Coding",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Rentals",
                newName: "CarId");

            migrationBuilder.RenameIndex(
                name: "IX_Rentals_VehicleId",
                table: "Rentals",
                newName: "IX_Rentals_CarId");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Maintenances",
                newName: "CarId");

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_VehicleId",
                table: "Maintenances",
                newName: "IX_Maintenances_CarId");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Cars",
                newName: "CarId");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
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
                value: new DateTime(2025, 11, 17, 17, 30, 22, 121, DateTimeKind.Local).AddTicks(5558));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 17, 30, 22, 121, DateTimeKind.Local).AddTicks(5648));

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "CarId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 17, 30, 22, 121, DateTimeKind.Local).AddTicks(5650));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Role" },
                values: new object[] { new DateTime(2025, 11, 17, 17, 30, 22, 120, DateTimeKind.Local).AddTicks(9695), "Admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Cars_CarId",
                table: "Maintenances",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Cars_CarId",
                table: "Rentals",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
