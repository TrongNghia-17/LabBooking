using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyRoomCheckLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents");

            migrationBuilder.DropTable(
                name: "EquipmentCheckResults");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "Incidents",
                newName: "RoomCheckId");

            migrationBuilder.RenameIndex(
                name: "IX_Incidents_SlotId",
                table: "Incidents",
                newName: "IX_Incidents_RoomCheckId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPassed",
                table: "RoomChecks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SlotId",
                table: "RoomChecks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomChecks_SlotId",
                table: "RoomChecks",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_RoomChecks_RoomCheckId",
                table: "Incidents",
                column: "RoomCheckId",
                principalTable: "RoomChecks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomChecks_Slots_SlotId",
                table: "RoomChecks",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_RoomChecks_RoomCheckId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomChecks_Slots_SlotId",
                table: "RoomChecks");

            migrationBuilder.DropIndex(
                name: "IX_RoomChecks_SlotId",
                table: "RoomChecks");

            migrationBuilder.DropColumn(
                name: "IsPassed",
                table: "RoomChecks");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "RoomChecks");

            migrationBuilder.RenameColumn(
                name: "RoomCheckId",
                table: "Incidents",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_Incidents_RoomCheckId",
                table: "Incidents",
                newName: "IX_Incidents_SlotId");

            migrationBuilder.CreateTable(
                name: "EquipmentCheckResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoomCheckId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOK = table.Column<bool>(type: "boolean", nullable: false),
                    IssueDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentCheckResults_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentCheckResults_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentCheckResults_RoomChecks_RoomCheckId",
                        column: x => x.RoomCheckId,
                        principalTable: "RoomChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCheckResults_EquipmentId",
                table: "EquipmentCheckResults",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCheckResults_IncidentId",
                table: "EquipmentCheckResults",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCheckResults_RoomCheckId",
                table: "EquipmentCheckResults",
                column: "RoomCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Slots_SlotId",
                table: "Incidents",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id");
        }
    }
}
