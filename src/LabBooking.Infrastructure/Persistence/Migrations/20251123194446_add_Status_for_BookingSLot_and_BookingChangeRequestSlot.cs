using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class add_Status_for_BookingSLot_and_BookingChangeRequestSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Bookings_BookingId",
                table: "Incidents");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Incidents",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_Incidents_BookingId",
                table: "Incidents",
                newName: "IX_Incidents_SlotId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BookingSlots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OriginalOverriddenSlotsJson",
                table: "BookingChangeRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "BookingChangeRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookingSlots");

            migrationBuilder.DropColumn(
                name: "OriginalOverriddenSlotsJson",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "BookingChangeRequests");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "Incidents",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Incidents_SlotId",
                table: "Incidents",
                newName: "IX_Incidents_BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Bookings_BookingId",
                table: "Incidents",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
