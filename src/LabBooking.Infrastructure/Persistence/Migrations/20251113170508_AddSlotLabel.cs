using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Slots",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Label",
                table: "Slots");
        }
    }
}
