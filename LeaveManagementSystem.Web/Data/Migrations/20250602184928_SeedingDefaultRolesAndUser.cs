using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LeaveManagementSystem.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingDefaultRolesAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0328D8E9-1801-4752-8C5B-9E1737FC94D4", null, "Supervisor", "SUPERVISOR" },
                    { "17104DE5-BE3E-4860-A8F2-DE664A5D2D28", null, "Employee", "EMPLOYEE" },
                    { "2C02596C-1392-41CE-9A2E-B131D33A2204", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5BE043C0-704E-481E-8FDC-F517E78BD469", 0, "32a8b1a9-fff5-4f92-9208-595c84076b2c", "admin@localhost.com", true, false, null, "ADMIN@LOCALHOST.COM", "ADMIN@LOCALHOST.COM", "AQAAAAIAAYagAAAAEAWWa+YBzq4vIukiefXR8FaVpsZ/6/c8QZ3eV3eBXLcaBzNcM5uSfFeLtnGKsJIFNA==", null, false, "5ecc6394-13a3-43df-ad2f-03f33788ad85", false, "admin@localhost.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "2C02596C-1392-41CE-9A2E-B131D33A2204", "5BE043C0-704E-481E-8FDC-F517E78BD469" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0328D8E9-1801-4752-8C5B-9E1737FC94D4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17104DE5-BE3E-4860-A8F2-DE664A5D2D28");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2C02596C-1392-41CE-9A2E-B131D33A2204", "5BE043C0-704E-481E-8FDC-F517E78BD469" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2C02596C-1392-41CE-9A2E-B131D33A2204");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5BE043C0-704E-481E-8FDC-F517E78BD469");
        }
    }
}
