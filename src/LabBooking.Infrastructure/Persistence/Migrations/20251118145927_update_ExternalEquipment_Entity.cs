using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class update_ExternalEquipment_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "ExternalEquipments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExternalEquipments_BookingId",
                table: "ExternalEquipments",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalEquipments_Bookings_BookingId",
                table: "ExternalEquipments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalEquipments_Bookings_BookingId",
                table: "ExternalEquipments");

            migrationBuilder.DropIndex(
                name: "IX_ExternalEquipments_BookingId",
                table: "ExternalEquipments");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "ExternalEquipments");
        }
    }
}
