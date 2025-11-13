using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class deleteparticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingParticipants");

            migrationBuilder.DropColumn(
                name: "NumberOf",
                table: "EquipmentMaintainSchedules");

            migrationBuilder.AddColumn<int>(
                name: "ImportanceLevel",
                table: "Incidents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportanceLevel",
                table: "Incidents");

            migrationBuilder.AddColumn<int>(
                name: "NumberOf",
                table: "EquipmentMaintainSchedules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingParticipants_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingParticipants_BookingId",
                table: "BookingParticipants",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingParticipants_UserId",
                table: "BookingParticipants",
                column: "UserId");
        }
    }
}
