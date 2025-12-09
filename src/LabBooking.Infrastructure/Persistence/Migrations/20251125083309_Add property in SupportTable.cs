using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddpropertyinSupportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents");

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "Supports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Supports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "Supports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Supports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                table: "Incidents",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

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
                name: "CreatedAt",
                table: "Supports");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "Supports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Supports");

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "Supports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

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
