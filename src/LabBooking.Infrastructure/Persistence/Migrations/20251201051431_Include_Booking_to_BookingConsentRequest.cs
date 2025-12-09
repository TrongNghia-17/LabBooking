using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class Include_Booking_to_BookingConsentRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "BookingConsentRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingConsentRequests_BookingId",
                table: "BookingConsentRequests",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConsentRequests_PriorityBookingId",
                table: "BookingConsentRequests",
                column: "PriorityBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingConsentRequests_Bookings_BookingId",
                table: "BookingConsentRequests",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingConsentRequests_Bookings_PriorityBookingId",
                table: "BookingConsentRequests",
                column: "PriorityBookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingConsentRequests_Bookings_BookingId",
                table: "BookingConsentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingConsentRequests_Bookings_PriorityBookingId",
                table: "BookingConsentRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingConsentRequests_BookingId",
                table: "BookingConsentRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingConsentRequests_PriorityBookingId",
                table: "BookingConsentRequests");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "BookingConsentRequests");
        }
    }
}
