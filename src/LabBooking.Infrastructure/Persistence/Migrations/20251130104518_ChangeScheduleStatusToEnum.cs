using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class ChangeScheduleStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
        @"ALTER TABLE ""EquipmentMaintainSchedules"" 
          ALTER COLUMN ""Status"" DROP DEFAULT;"
    );

            // BƯỚC 2: Đổi kiểu dữ liệu sang Integer và Map dữ liệu cũ
            migrationBuilder.Sql(
                @"ALTER TABLE ""EquipmentMaintainSchedules"" 
          ALTER COLUMN ""Status"" TYPE integer 
          USING (
            CASE 
                WHEN ""Status"" = 'Done' THEN 1 
                ELSE 0 
            END
          )::integer;"
            );

            // BƯỚC 3: Thiết lập giá trị mặc định mới (là số 0) và set NOT NULL
            migrationBuilder.Sql(
                @"ALTER TABLE ""EquipmentMaintainSchedules"" 
          ALTER COLUMN ""Status"" SET DEFAULT 0;"
            );

            migrationBuilder.Sql(
                @"ALTER TABLE ""EquipmentMaintainSchedules"" 
          ALTER COLUMN ""Status"" SET NOT NULL;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EquipmentMaintainSchedules",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
