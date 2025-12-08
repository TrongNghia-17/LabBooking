using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIncidentOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Equipments_EquipmentId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_EquipmentId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Incidents");

            migrationBuilder.CreateTable(
                name: "IncidentEquipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentEquipments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEquipments_EquipmentId",
                table: "IncidentEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEquipments_IncidentId",
                table: "IncidentEquipments",
                column: "IncidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentEquipments");

            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentId",
                table: "Incidents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EquipmentId",
                table: "Incidents",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Equipments_EquipmentId",
                table: "Incidents",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");
        }
    }
}
