using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Equipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Supports_CreatedById",
                table: "Supports",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Supports_AspNetUsers_CreatedById",
                table: "Supports",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supports_AspNetUsers_CreatedById",
                table: "Supports");

            migrationBuilder.DropIndex(
                name: "IX_Supports_CreatedById",
                table: "Supports");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Equipments");
        }
    }
}
