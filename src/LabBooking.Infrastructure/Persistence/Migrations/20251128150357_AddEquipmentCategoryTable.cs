using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "EquipmentCategories",
        columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "text", nullable: false),
            Description = table.Column<string>(type: "text", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_EquipmentCategories", x => x.Id);
        });

            var defaultCategoryId = Guid.NewGuid();

            migrationBuilder.Sql($"INSERT INTO \"EquipmentCategories\" (\"Id\", \"Name\", \"Description\") VALUES ('{defaultCategoryId}', 'General', 'Default category for existing equipments')");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Equipments");

            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentCategoryId",
                table: "Equipments",
                type: "uuid",
                nullable: false,
                defaultValue: defaultCategoryId);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentCategoryId",
                table: "Equipments",
                column: "EquipmentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentCategories_EquipmentCategoryId",
                table: "Equipments",
                column: "EquipmentCategoryId",
                principalTable: "EquipmentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentCategories_EquipmentCategoryId",
                table: "Equipments");

            migrationBuilder.DropTable(
                name: "EquipmentCategories");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_EquipmentCategoryId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "EquipmentCategoryId",
                table: "Equipments");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Equipments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
