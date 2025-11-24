using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class add_OverriddenByBookingId_prop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingConsentRequests_BookingSlots_ConflictingSlotId",
                table: "BookingConsentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingConsentRequests_Bookings_NewBookingId",
                table: "BookingConsentRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingConsentRequests_ConflictingSlotId",
                table: "BookingConsentRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingConsentRequests_NewBookingId",
                table: "BookingConsentRequests");

            migrationBuilder.DropColumn(
                name: "ResponseDate",
                table: "BookingConsentRequests");

            migrationBuilder.RenameColumn(
                name: "OriginalOwnerId",
                table: "BookingConsentRequests",
                newName: "PriorityBookingId");

            migrationBuilder.RenameColumn(
                name: "NewBookingId",
                table: "BookingConsentRequests",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "ConflictingSlotId",
                table: "BookingConsentRequests",
                newName: "BookingId");

            migrationBuilder.AddColumn<Guid>(
                name: "OverriddenByBookingId",
                table: "BookingSlots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverriddenSlotIdsJson",
                table: "BookingConsentRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverriddenByBookingId",
                table: "BookingSlots");

            migrationBuilder.DropColumn(
                name: "OverriddenSlotIdsJson",
                table: "BookingConsentRequests");

            migrationBuilder.RenameColumn(
                name: "PriorityBookingId",
                table: "BookingConsentRequests",
                newName: "OriginalOwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "BookingConsentRequests",
                newName: "NewBookingId");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "BookingConsentRequests",
                newName: "ConflictingSlotId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDate",
                table: "BookingConsentRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingConsentRequests_ConflictingSlotId",
                table: "BookingConsentRequests",
                column: "ConflictingSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConsentRequests_NewBookingId",
                table: "BookingConsentRequests",
                column: "NewBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingConsentRequests_BookingSlots_ConflictingSlotId",
                table: "BookingConsentRequests",
                column: "ConflictingSlotId",
                principalTable: "BookingSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingConsentRequests_Bookings_NewBookingId",
                table: "BookingConsentRequests",
                column: "NewBookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
