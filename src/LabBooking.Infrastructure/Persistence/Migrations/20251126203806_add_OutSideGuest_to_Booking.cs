using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class add_OutSideGuest_to_Booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents");

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                table: "Incidents",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "NewOutSideGuestsJson",
                table: "BookingChangeRequests",
                type: "text",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_OutSideGuests_BookingId",
            //    table: "OutSideGuests",
            //    column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_OutSideGuests_Bookings_BookingId",
            //    table: "OutSideGuests",
            //    column: "BookingId",
            //    principalTable: "Bookings",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_OutSideGuests_Bookings_BookingId",
            //    table: "OutSideGuests");

            //migrationBuilder.DropIndex(
            //    name: "IX_OutSideGuests_BookingId",
            //    table: "OutSideGuests");

            migrationBuilder.DropColumn(
                name: "NewOutSideGuestsJson",
                table: "BookingChangeRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                table: "Incidents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
