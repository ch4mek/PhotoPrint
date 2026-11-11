using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Booking_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.EnsureSchema(
                name: "booking");

            migrationBuilder.CreateTable(
                name: "BookingUsers",
                schema: "booking",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_BookingUsers_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Offices",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TimeZoneDisplayName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offices_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offices_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingRooms",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CountOfSeats = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    OfficeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingRooms_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRooms_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRooms_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "booking",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    InternalCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    IsAvailableForBooking = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OfficeId = table.Column<int>(type: "integer", nullable: false),
                    IsClaimRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Workspaces_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Workspaces_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "booking",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingRoomEntries",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RepeatEveryWeek = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ReservationFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReservationToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReservationPeriodFromOfficeDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReservationPeriodToOfficeDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MeetingRoomId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRoomEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingRoomEntries_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomEntries_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomEntries_BookingUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomEntries_MeetingRooms_MeetingRoomId",
                        column: x => x.MeetingRoomId,
                        principalSchema: "booking",
                        principalTable: "MeetingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingRoomItems",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ItemType = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    MeetingRoomId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRoomItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingRoomItems_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomItems_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomItems_MeetingRooms_MeetingRoomId",
                        column: x => x.MeetingRoomId,
                        principalSchema: "booking",
                        principalTable: "MeetingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workplaces",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkplaceNumber = table.Column<int>(type: "integer", nullable: false),
                    OfficeId = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workplaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workplaces_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Workplaces_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Workplaces_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "booking",
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workplaces_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "booking",
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MeetingRoomReservations",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MeetingRoomEntryId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ReservationFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReservationToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Comments = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRoomReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingRoomReservations_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomReservations_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomReservations_BookingUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeetingRoomReservations_MeetingRoomEntries_MeetingRoomEntry~",
                        column: x => x.MeetingRoomEntryId,
                        principalSchema: "booking",
                        principalTable: "MeetingRoomEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingEntries",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsBlock = table.Column<bool>(type: "boolean", nullable: false),
                    WeeksInInterval = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsForGuest = table.Column<bool>(type: "boolean", nullable: false),
                    GuestName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    WorkplaceId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingEntries_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_BookingEntries_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_BookingEntries_BookingUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingEntries_Workplaces_WorkplaceId",
                        column: x => x.WorkplaceId,
                        principalSchema: "booking",
                        principalTable: "Workplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingNotifications",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingEntryId = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    WasRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingNotifications_BookingEntries_BookingEntryId",
                        column: x => x.BookingEntryId,
                        principalSchema: "booking",
                        principalTable: "BookingEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingNotifications_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_BookingNotifications_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReservationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReservationYear = table.Column<int>(type: "integer", nullable: false),
                    ReservationForMonth = table.Column<int>(type: "integer", nullable: false),
                    ReservationForWeek = table.Column<int>(type: "integer", nullable: false),
                    ReservationForDay = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    BookingEntryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_BookingEntries_BookingEntryId",
                        column: x => x.BookingEntryId,
                        principalSchema: "booking",
                        principalTable: "BookingEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_BookingUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Reservations_BookingUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "booking",
                        principalTable: "BookingUsers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingEntries_CreatedById",
                schema: "booking",
                table: "BookingEntries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingEntries_IsDeleted_WorkplaceId_GuestName",
                schema: "booking",
                table: "BookingEntries",
                columns: ["IsDeleted", "WorkplaceId", "GuestName"]);

            migrationBuilder.CreateIndex(
                name: "IX_BookingEntries_UpdatedById",
                schema: "booking",
                table: "BookingEntries",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingEntries_UserId",
                schema: "booking",
                table: "BookingEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingEntries_WorkplaceId",
                schema: "booking",
                table: "BookingEntries",
                column: "WorkplaceId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingNotifications_BookingEntryId",
                schema: "booking",
                table: "BookingNotifications",
                column: "BookingEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingNotifications_CreatedById",
                schema: "booking",
                table: "BookingNotifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingNotifications_UpdatedById",
                schema: "booking",
                table: "BookingNotifications",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookingNotifications_WasRead",
                schema: "booking",
                table: "BookingNotifications",
                column: "WasRead");

            migrationBuilder.CreateIndex(
                name: "IX_BookingUsers_Email",
                schema: "booking",
                table: "BookingUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_BookingUsers_IsActive",
                schema: "booking",
                table: "BookingUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BookingUsers_UpdatedById",
                schema: "booking",
                table: "BookingUsers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomEntries_CreatedById",
                schema: "booking",
                table: "MeetingRoomEntries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomEntries_IsDeleted_RepeatEveryWeek",
                schema: "booking",
                table: "MeetingRoomEntries",
                columns: ["IsDeleted", "RepeatEveryWeek"]);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomEntries_MeetingRoomId",
                schema: "booking",
                table: "MeetingRoomEntries",
                column: "MeetingRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomEntries_UpdatedById",
                schema: "booking",
                table: "MeetingRoomEntries",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomEntries_UserId",
                schema: "booking",
                table: "MeetingRoomEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomItems_CreatedById",
                schema: "booking",
                table: "MeetingRoomItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomItems_MeetingRoomId",
                schema: "booking",
                table: "MeetingRoomItems",
                column: "MeetingRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomItems_UpdatedById",
                schema: "booking",
                table: "MeetingRoomItems",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_CreatedById",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_IsDeleted",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_MeetingRoomEntryId",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "MeetingRoomEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_ReservationFromUtc",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "ReservationFromUtc");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_ReservationToUtc",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "ReservationToUtc");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_UpdatedById",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRoomReservations_UserId",
                schema: "booking",
                table: "MeetingRoomReservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRooms_CreatedById",
                schema: "booking",
                table: "MeetingRooms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRooms_OfficeId_Name",
                schema: "booking",
                table: "MeetingRooms",
                columns: ["OfficeId", "Name"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRooms_UpdatedById",
                schema: "booking",
                table: "MeetingRooms",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_CreatedById",
                schema: "booking",
                table: "Offices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_UpdatedById",
                schema: "booking",
                table: "Offices",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookingEntryId",
                schema: "booking",
                table: "Reservations",
                column: "BookingEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CreatedById",
                schema: "booking",
                table: "Reservations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UpdatedById",
                schema: "booking",
                table: "Reservations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_CreatedById",
                schema: "booking",
                table: "Workplaces",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_OfficeId_WorkspaceId_WorkplaceNumber",
                schema: "booking",
                table: "Workplaces",
                columns: ["OfficeId", "WorkspaceId", "WorkplaceNumber"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_UpdatedById",
                schema: "booking",
                table: "Workplaces",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_WorkspaceId",
                schema: "booking",
                table: "Workplaces",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_CreatedById",
                schema: "booking",
                table: "Workspaces",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_OfficeId_Name",
                schema: "booking",
                table: "Workspaces",
                columns: ["OfficeId", "Name"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UpdatedById",
                schema: "booking",
                table: "Workspaces",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropTable(
                name: "BookingNotifications",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "MeetingRoomItems",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "MeetingRoomReservations",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "MeetingRoomEntries",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "BookingEntries",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "MeetingRooms",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Workplaces",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Workspaces",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Offices",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "BookingUsers",
                schema: "booking");
        }
    }
}
