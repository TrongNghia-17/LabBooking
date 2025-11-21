using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class update_Booking_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPriorityDetails_AspNetUsers_ApprovedById",
                table: "BookingPriorityDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingPriorityDetails_Bookings_BookingId",
                table: "BookingPriorityDetails");

            migrationBuilder.DropIndex(
                name: "IX_BookingPriorityDetails_ApprovedById",
                table: "BookingPriorityDetails");

            migrationBuilder.DropIndex(
                name: "IX_BookingPriorityDetails_BookingId",
                table: "BookingPriorityDetails");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "BookingPriorityDetails");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "BookingPriorityDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "Bookings",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Bookings");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "BookingPriorityDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "BookingPriorityDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPriorityDetails_ApprovedById",
                table: "BookingPriorityDetails",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPriorityDetails_BookingId",
                table: "BookingPriorityDetails",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPriorityDetails_AspNetUsers_ApprovedById",
                table: "BookingPriorityDetails",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPriorityDetails_Bookings_BookingId",
                table: "BookingPriorityDetails",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
