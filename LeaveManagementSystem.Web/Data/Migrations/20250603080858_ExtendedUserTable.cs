using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagementSystem.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5BE043C0-704E-481E-8FDC-F517E78BD469",
                columns: new[] { "ConcurrencyStamp", "DateOfBirth", "FirstName", "LastName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9de34899-a507-43ef-9f8d-5139c05951e2", new DateOnly(1990, 1, 1), "Default", "Admin", "AQAAAAIAAYagAAAAEMZYUg6Qr6ejcuh5rn/eyA5QqKc3eItBiPHthJKAHgc14wjNp/BtdWtMZks6aaUiXQ==", "aa96e410-9f88-4e36-93ba-17e479418b1a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5BE043C0-704E-481E-8FDC-F517E78BD469",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "32a8b1a9-fff5-4f92-9208-595c84076b2c", "AQAAAAIAAYagAAAAEAWWa+YBzq4vIukiefXR8FaVpsZ/6/c8QZ3eV3eBXLcaBzNcM5uSfFeLtnGKsJIFNA==", "5ecc6394-13a3-43df-ad2f-03f33788ad85" });
        }
    }
}
