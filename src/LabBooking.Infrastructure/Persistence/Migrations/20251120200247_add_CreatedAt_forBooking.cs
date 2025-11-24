using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class add_CreatedAt_forBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingChangeRequests_AspNetUsers_ApprovedById",
                table: "BookingChangeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingChangeRequests_AspNetUsers_CreatedById",
                table: "BookingChangeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingChangeRequests_BookingSlots_NewBookingSlotId",
                table: "BookingChangeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingChangeRequests_BookingSlots_OldBookingSlotId",
                table: "BookingChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingChangeRequests_ApprovedById",
                table: "BookingChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingChangeRequests_CreatedById",
                table: "BookingChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingChangeRequests_NewBookingSlotId",
                table: "BookingChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingChangeRequests_OldBookingSlotId",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExternalEquipments");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "NewBookingSlotId",
                table: "BookingChangeRequests");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "BookingChangeRequests",
                newName: "NewTitle");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "BookingChangeRequests",
                newName: "NewProjectJson");

            migrationBuilder.RenameColumn(
                name: "OldBookingSlotId",
                table: "BookingChangeRequests",
                newName: "RequestedById");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BookingChangeRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BookingChangeRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ManagerReason",
                table: "BookingChangeRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewCourseId",
                table: "BookingChangeRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewDescription",
                table: "BookingChangeRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewExternalEquipmentsJson",
                table: "BookingChangeRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewNumberOfParticipants",
                table: "BookingChangeRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewPriorityDetailJson",
                table: "BookingChangeRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "BookingChangeRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "BookingChangeRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingChangeRequestSlot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingChangeRequestSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingChangeRequestSlot_BookingChangeRequests_BookingChang~",
                        column: x => x.BookingChangeRequestId,
                        principalTable: "BookingChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequestSlot_BookingChangeRequestId",
                table: "BookingChangeRequestSlot",
                column: "BookingChangeRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingChangeRequestSlot");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "ManagerReason",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "NewCourseId",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "NewDescription",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "NewExternalEquipmentsJson",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "NewNumberOfParticipants",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "NewPriorityDetailJson",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "BookingChangeRequests");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "BookingChangeRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedById",
                table: "BookingChangeRequests",
                newName: "OldBookingSlotId");

            migrationBuilder.RenameColumn(
                name: "NewTitle",
                table: "BookingChangeRequests",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "NewProjectJson",
                table: "BookingChangeRequests",
                newName: "Reason");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ExternalEquipments",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BookingChangeRequests",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "BookingChangeRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "BookingChangeRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "NewBookingSlotId",
                table: "BookingChangeRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequests_ApprovedById",
                table: "BookingChangeRequests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequests_CreatedById",
                table: "BookingChangeRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequests_NewBookingSlotId",
                table: "BookingChangeRequests",
                column: "NewBookingSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequests_OldBookingSlotId",
                table: "BookingChangeRequests",
                column: "OldBookingSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingChangeRequests_AspNetUsers_ApprovedById",
                table: "BookingChangeRequests",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingChangeRequests_AspNetUsers_CreatedById",
                table: "BookingChangeRequests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingChangeRequests_BookingSlots_NewBookingSlotId",
                table: "BookingChangeRequests",
                column: "NewBookingSlotId",
                principalTable: "BookingSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingChangeRequests_BookingSlots_OldBookingSlotId",
                table: "BookingChangeRequests",
                column: "OldBookingSlotId",
                principalTable: "BookingSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
