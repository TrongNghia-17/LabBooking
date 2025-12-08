using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentIdToIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
