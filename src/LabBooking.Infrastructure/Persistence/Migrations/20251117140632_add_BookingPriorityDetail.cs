using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class add_BookingPriorityDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingPriorityDetailId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingPriorityDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Justification = table.Column<string>(type: "text", nullable: false),
                    EvidenceFilePath = table.Column<string>(type: "text", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ManagerNote = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPriorityDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPriorityDetails_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookingPriorityDetails_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingPriorityDetailId",
                table: "Bookings",
                column: "BookingPriorityDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPriorityDetails_ApprovedById",
                table: "BookingPriorityDetails",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPriorityDetails_BookingId",
                table: "BookingPriorityDetails",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BookingPriorityDetails_BookingPriorityDetailId",
                table: "Bookings",
                column: "BookingPriorityDetailId",
                principalTable: "BookingPriorityDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BookingPriorityDetails_BookingPriorityDetailId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "BookingPriorityDetails");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingPriorityDetailId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingPriorityDetailId",
                table: "Bookings");
        }
    }
}
