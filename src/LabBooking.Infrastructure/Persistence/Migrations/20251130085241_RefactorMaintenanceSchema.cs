using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class RefactorMaintenanceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMaintainSchedules_Equipments_EquipmentId",
                table: "EquipmentMaintainSchedules");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentMaintainSchedules_EquipmentId",
                table: "EquipmentMaintainSchedules");

            migrationBuilder.DropColumn(
                name: "EquimentpMaintainStatus",
                table: "EquipmentMaintainSchedules");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "EquipmentMaintainSchedules");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EquipmentMaintainSchedules",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EquipmentMaintainSchedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EquipmentMaintenances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentMaintainScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResultNote = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentMaintenances_EquipmentMaintainSchedules_EquipmentM~",
                        column: x => x.EquipmentMaintainScheduleId,
                        principalTable: "EquipmentMaintainSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentMaintenances_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintenances_EquipmentId",
                table: "EquipmentMaintenances",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintenances_EquipmentMaintainScheduleId",
                table: "EquipmentMaintenances",
                column: "EquipmentMaintainScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentMaintenances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EquipmentMaintainSchedules");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EquipmentMaintainSchedules",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "EquimentpMaintainStatus",
                table: "EquipmentMaintainSchedules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentId",
                table: "EquipmentMaintainSchedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintainSchedules_EquipmentId",
                table: "EquipmentMaintainSchedules",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMaintainSchedules_Equipments_EquipmentId",
                table: "EquipmentMaintainSchedules",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
