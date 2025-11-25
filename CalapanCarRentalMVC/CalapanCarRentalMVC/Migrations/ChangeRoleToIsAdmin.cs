using Microsoft.EntityFrameworkCore.Migrations;

namespace CalapanCarRentalMVC.Migrations
{
 public partial class ChangeRoleToIsAdmin : Migration
 {
 protected override void Up(MigrationBuilder migrationBuilder)
 {
 // Rename column
 migrationBuilder.RenameColumn(
 name: "Role",
 table: "Users",
 newName: "is_Admin");

 // Alter column type to tinyint (byte)
 migrationBuilder.AlterColumn<byte>(
 name: "is_Admin",
 table: "Users",
 nullable: false,
 oldClrType: typeof(string),
 oldType: "nvarchar(50)");

 // Update data:0 = Admin,1 = Customer
 migrationBuilder.Sql("UPDATE Users SET is_Admin =0 WHERE is_Admin = 'Admin'");
 migrationBuilder.Sql("UPDATE Users SET is_Admin =1 WHERE is_Admin = 'Customer'");
 }

 protected override void Down(MigrationBuilder migrationBuilder)
 {
 // Revert data
 migrationBuilder.Sql("UPDATE Users SET is_Admin = 'Admin' WHERE is_Admin =0");
 migrationBuilder.Sql("UPDATE Users SET is_Admin = 'Customer' WHERE is_Admin =1");

 // Change type back to string
 migrationBuilder.AlterColumn<string>(
 name: "is_Admin",
 table: "Users",
 type: "nvarchar(50)",
 nullable: false,
 oldClrType: typeof(byte));

 // Rename column back
 migrationBuilder.RenameColumn(
 name: "is_Admin",
 table: "Users",
 newName: "Role");
 }
 }
}
