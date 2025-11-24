using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class deleteslotIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlotIndex",
                table: "BookingSlots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SlotIndex",
                table: "BookingSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
