using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Role_Table_Mapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c7b013f0-5201-4317-abd8-c211f91b7330"), null, "Teacher", "TEACHER" },
                    { new Guid("fab4fac1-c546-41de-aebc-a14da6895711"), null, "Student", "STUDENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c7b013f0-5201-4317-abd8-c211f91b7330"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fab4fac1-c546-41de-aebc-a14da6895711"));

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    NormalizedName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c7b013f0-5201-4317-abd8-c211f91b7330", null, "Teacher", "TEACHER" },
                    { "fab4fac1-c546-41de-aebc-a14da6895711", null, "Student", "STUDENT" }
                });
        }
    }
}
