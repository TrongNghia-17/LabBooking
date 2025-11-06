using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabBooking.Application.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_CreatedById",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AspNetUsers_ReportedById",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRooms_AspNetUsers_MainManagerId",
                table: "LabRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDevice_AspNetUsers_UserId",
                table: "UserDevice");

            migrationBuilder.DropTable(
                name: "DoorRequests");

            migrationBuilder.DropTable(
                name: "ProjectMembers");

            migrationBuilder.DropIndex(
                name: "IX_BookingSlots_Date_SlotIndex_BookingId",
                table: "BookingSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDevice",
                table: "UserDevice");

            migrationBuilder.DropIndex(
                name: "IX_UserDevice_PushToken",
                table: "UserDevice");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c7b013f0-5201-4317-abd8-c211f91b7330"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fab4fac1-c546-41de-aebc-a14da6895711"));

            migrationBuilder.DropColumn(
                name: "ClassCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CourseCode",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "UserDevice",
                newName: "UserDevices");

            migrationBuilder.RenameColumn(
                name: "LabRoomId1",
                table: "Bookings",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_LabRoomId1",
                table: "Bookings",
                newName: "IX_Bookings_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDevice_UserId",
                table: "UserDevices",
                newName: "IX_UserDevices_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "LabRooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LabRooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LabRooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "LabRooms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumLimit",
                table: "LabRooms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Incidents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Equipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SlotId",
                table: "BookingSlots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Bookings",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Bookings",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMajorOnly",
                table: "Bookings",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfParticipants",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BookingParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDevices",
                table: "UserDevices",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BookingChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewBookingSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldBookingSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingChangeRequests_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingChangeRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingChangeRequests_BookingSlots_NewBookingSlotId",
                        column: x => x.NewBookingSlotId,
                        principalTable: "BookingSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingChangeRequests_BookingSlots_OldBookingSlotId",
                        column: x => x.OldBookingSlotId,
                        principalTable: "BookingSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingChangeRequests_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseCode = table.Column<string>(type: "text", nullable: true),
                    CourseName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoorOpeningRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    HandledById = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorOpeningRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoorOpeningRequests_AspNetUsers_HandledById",
                        column: x => x.HandledById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoorOpeningRequests_AspNetUsers_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoorOpeningRequests_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoorOpeningRequests_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMaintainSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsManyDay = table.Column<bool>(type: "boolean", nullable: false),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NumberOfSlot = table.Column<int>(type: "integer", nullable: true),
                    EquimentpMaintainStatus = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    NumberOf = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMaintainSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentMaintainSchedules_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalEquipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalEquipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutSideGuests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Organization = table.Column<string>(type: "text", nullable: true),
                    PurposeOfVisit = table.Column<string>(type: "text", nullable: true),
                    VisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSideGuests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutSideGuests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomMaintainSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsManyDay = table.Column<bool>(type: "boolean", nullable: false),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NumberOfSlot = table.Column<int>(type: "integer", nullable: true),
                    RoomMaintainStatus = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomMaintainSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomMaintainSchedules_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SlotIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsagePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ForAllLabRooms = table.Column<bool>(type: "boolean", nullable: false),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsagePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsagePolicies_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabRooms_CreatedById",
                table: "LabRooms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_BookingId",
                table: "Incidents",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSlots_SlotId",
                table: "BookingSlots",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequests_ApprovedById",
                table: "BookingChangeRequests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingChangeRequests_BookingId",
                table: "BookingChangeRequests",
                column: "BookingId");

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

            migrationBuilder.CreateIndex(
                name: "IX_DoorOpeningRequests_BookingId",
                table: "DoorOpeningRequests",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_DoorOpeningRequests_HandledById",
                table: "DoorOpeningRequests",
                column: "HandledById");

            migrationBuilder.CreateIndex(
                name: "IX_DoorOpeningRequests_LabRoomId",
                table: "DoorOpeningRequests",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_DoorOpeningRequests_RequestedById",
                table: "DoorOpeningRequests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintainSchedules_EquipmentId",
                table: "EquipmentMaintainSchedules",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSideGuests_CreatedById",
                table: "OutSideGuests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMaintainSchedules_LabRoomId",
                table: "RoomMaintainSchedules",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UsagePolicies_CreatedById",
                table: "UsagePolicies",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_CreatedById",
                table: "Bookings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courses_CourseId",
                table: "Bookings",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId",
                table: "Bookings",
                column: "LabRoomId",
                principalTable: "LabRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSlots_Slots_SlotId",
                table: "BookingSlots",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AspNetUsers_ReportedById",
                table: "Incidents",
                column: "ReportedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Bookings_BookingId",
                table: "Incidents",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRooms_AspNetUsers_CreatedById",
                table: "LabRooms",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LabRooms_AspNetUsers_MainManagerId",
                table: "LabRooms",
                column: "MainManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevices_AspNetUsers_UserId",
                table: "UserDevices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_CreatedById",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courses_CourseId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSlots_Slots_SlotId",
                table: "BookingSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AspNetUsers_ReportedById",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Bookings_BookingId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRooms_AspNetUsers_CreatedById",
                table: "LabRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRooms_AspNetUsers_MainManagerId",
                table: "LabRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDevices_AspNetUsers_UserId",
                table: "UserDevices");

            migrationBuilder.DropTable(
                name: "BookingChangeRequests");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "DoorOpeningRequests");

            migrationBuilder.DropTable(
                name: "EquipmentMaintainSchedules");

            migrationBuilder.DropTable(
                name: "ExternalEquipments");

            migrationBuilder.DropTable(
                name: "OutSideGuests");

            migrationBuilder.DropTable(
                name: "RoomMaintainSchedules");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "Supports");

            migrationBuilder.DropTable(
                name: "UsagePolicies");

            migrationBuilder.DropIndex(
                name: "IX_LabRooms_CreatedById",
                table: "LabRooms");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_BookingId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_BookingSlots_SlotId",
                table: "BookingSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDevices",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "MaximumLimit",
                table: "LabRooms");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "BookingSlots");

            migrationBuilder.DropColumn(
                name: "NumberOfParticipants",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "BookingParticipants");

            migrationBuilder.RenameTable(
                name: "UserDevices",
                newName: "UserDevice");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Bookings",
                newName: "LabRoomId1");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_CourseId",
                table: "Bookings",
                newName: "IX_Bookings_LabRoomId1");

            migrationBuilder.RenameIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevice",
                newName: "IX_UserDevice_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMajorOnly",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassCode",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseCode",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDevice",
                table: "UserDevice",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DoorRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HandledById = table.Column<Guid>(type: "uuid", nullable: true),
                    LabRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoorRequests_AspNetUsers_HandledById",
                        column: x => x.HandledById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoorRequests_AspNetUsers_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoorRequests_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    MembersId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => new { x.MembersId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectMembers_AspNetUsers_MembersId",
                        column: x => x.MembersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c7b013f0-5201-4317-abd8-c211f91b7330"), "16056a26-161a-4540-962d-c90c4d5e74d9", "Teacher", "TEACHER" },
                    { new Guid("fab4fac1-c546-41de-aebc-a14da6895711"), "d69a2e0f-751c-44b5-aae8-2579faa48b9e", "Student", "STUDENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSlots_Date_SlotIndex_BookingId",
                table: "BookingSlots",
                columns: new[] { "Date", "SlotIndex", "BookingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevice_PushToken",
                table: "UserDevice",
                column: "PushToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoorRequests_HandledById",
                table: "DoorRequests",
                column: "HandledById");

            migrationBuilder.CreateIndex(
                name: "IX_DoorRequests_LabRoomId",
                table: "DoorRequests",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_DoorRequests_RequestedById",
                table: "DoorRequests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_CreatedById",
                table: "Bookings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId",
                table: "Bookings",
                column: "LabRoomId",
                principalTable: "LabRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_LabRooms_LabRoomId1",
                table: "Bookings",
                column: "LabRoomId1",
                principalTable: "LabRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AspNetUsers_ReportedById",
                table: "Incidents",
                column: "ReportedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRooms_AspNetUsers_MainManagerId",
                table: "LabRooms",
                column: "MainManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevice_AspNetUsers_UserId",
                table: "UserDevice",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
