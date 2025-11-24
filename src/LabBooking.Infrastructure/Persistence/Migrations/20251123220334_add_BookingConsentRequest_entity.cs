using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class add_BookingConsentRequest_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingConsentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NewBookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConflictingSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingConsentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingConsentRequests_BookingSlots_ConflictingSlotId",
                        column: x => x.ConflictingSlotId,
                        principalTable: "BookingSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingConsentRequests_Bookings_NewBookingId",
                        column: x => x.NewBookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingConsentRequests_ConflictingSlotId",
                table: "BookingConsentRequests",
                column: "ConflictingSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConsentRequests_NewBookingId",
                table: "BookingConsentRequests",
                column: "NewBookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingConsentRequests");
        }
    }
}
