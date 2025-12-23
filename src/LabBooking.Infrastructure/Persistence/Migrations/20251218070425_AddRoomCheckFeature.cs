using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomCheckFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuardId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomChecks_AspNetUsers_GuardId",
                        column: x => x.GuardId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomChecks_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCheckResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomCheckId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOK = table.Column<bool>(type: "boolean", nullable: false),
                    IssueDescription = table.Column<string>(type: "text", nullable: true),
                    IncidentId = table.Column<Guid>(type: "uuid", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_RoomChecks_GuardId",
                table: "RoomChecks",
                column: "GuardId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomChecks_LabRoomId",
                table: "RoomChecks",
                column: "LabRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentCheckResults");

            migrationBuilder.DropTable(
                name: "RoomChecks");
        }
    }
}
