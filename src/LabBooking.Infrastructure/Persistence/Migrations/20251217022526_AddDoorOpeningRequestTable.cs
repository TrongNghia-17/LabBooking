using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddDoorOpeningRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoorOpeningRequests_AspNetUsers_HandledById",
                table: "DoorOpeningRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DoorOpeningRequests_Bookings_BookingId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_DoorOpeningRequests_LabRooms_LabRoomId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropIndex(
                name: "IX_DoorOpeningRequests_BookingId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropIndex(
                name: "IX_DoorOpeningRequests_LabRoomId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropColumn(
                name: "LabRoomId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DoorOpeningRequests");

            migrationBuilder.RenameColumn(
                name: "HandledById",
                table: "DoorOpeningRequests",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_DoorOpeningRequests_HandledById",
                table: "DoorOpeningRequests",
                newName: "IX_DoorOpeningRequests_ManagerId");

            migrationBuilder.AddColumn<string>(
                name: "BookingCode",
                table: "DoorOpeningRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "DoorOpeningRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DoorOpeningRequests_AspNetUsers_ManagerId",
                table: "DoorOpeningRequests",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoorOpeningRequests_AspNetUsers_ManagerId",
                table: "DoorOpeningRequests");

            migrationBuilder.DropColumn(
                name: "BookingCode",
                table: "DoorOpeningRequests");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "DoorOpeningRequests");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "DoorOpeningRequests",
                newName: "HandledById");

            migrationBuilder.RenameIndex(
                name: "IX_DoorOpeningRequests_ManagerId",
                table: "DoorOpeningRequests",
                newName: "IX_DoorOpeningRequests_HandledById");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "DoorOpeningRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LabRoomId",
                table: "DoorOpeningRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DoorOpeningRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DoorOpeningRequests_BookingId",
                table: "DoorOpeningRequests",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_DoorOpeningRequests_LabRoomId",
                table: "DoorOpeningRequests",
                column: "LabRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoorOpeningRequests_AspNetUsers_HandledById",
                table: "DoorOpeningRequests",
                column: "HandledById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoorOpeningRequests_Bookings_BookingId",
                table: "DoorOpeningRequests",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoorOpeningRequests_LabRooms_LabRoomId",
                table: "DoorOpeningRequests",
                column: "LabRoomId",
                principalTable: "LabRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
