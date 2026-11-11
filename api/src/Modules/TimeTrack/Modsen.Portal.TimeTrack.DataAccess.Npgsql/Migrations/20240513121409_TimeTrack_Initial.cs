using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class TimeTrack_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "timetrack");

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JsonData = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    WasSent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportRequests",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SizeInKilobytes = table.Column<double>(type: "double precision", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestIsValidTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedById = table.Column<int>(type: "integer", nullable: false),
                    ReportFileId = table.Column<string>(type: "text", nullable: false),
                    FilterHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FilterArgs = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeTrackResourceGroups",
                schema: "timetrack",
                columns: table => new
                {
                    ResourceGroupId = table.Column<int>(type: "integer", maxLength: 64, nullable: false),
                    ResourceGroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTrackResourceGroups", x => x.ResourceGroupId);
                });

            migrationBuilder.CreateTable(
                name: "TimeTrackUsers",
                schema: "timetrack",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    AvatarFileId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTrackUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_TimeTrackUsers_TimeTrackResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackResourceGroups",
                        principalColumn: "ResourceGroupId");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    Comments = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RegisteredLocation = table.Column<string>(type: "text", nullable: false),
                    SalesManagerId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_TimeTrackUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_TimeTrackUsers_SalesManagerId",
                        column: x => x.SalesManagerId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_TimeTrackUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimesheetSubmitStatuses",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DecisionBy = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    FactHoursSum = table.Column<int>(type: "integer", nullable: false),
                    BillableHoursSum = table.Column<int>(type: "integer", nullable: false),
                    Snapshot = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetSubmitStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimesheetSubmitStatuses_TimeTrackUsers_DecisionBy",
                        column: x => x.DecisionBy,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimesheetSubmitStatuses_TimeTrackUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsBillable = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TechStack = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Industry = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Architecture = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CloudServices = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ManagerUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "timetrack",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_TimeTrackUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_TimeTrackUsers_ManagerUserId",
                        column: x => x.ManagerUserId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_TimeTrackUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserOnProjectFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    UserOnProjectTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsTimeOnProjectBillable = table.Column<bool>(type: "boolean", nullable: false),
                    Fte = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    RefUserId = table.Column<int>(type: "integer", nullable: true),
                    Comments = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "timetrack",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TimeTrackUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TimeTrackUsers_RefUserId",
                        column: x => x.RefUserId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TimeTrackUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TimeTrackUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Timesheets",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FactTrackedInMinutes = table.Column<int>(type: "integer", nullable: false),
                    BillableTrackedInMinutes = table.Column<int>(type: "integer", nullable: false),
                    ReportForDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OrderByDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, computedColumnSql: "COALESCE(\"UpdatedAtUtc\", \"CreatedAtUtc\")", stored: true),
                    ReportYear = table.Column<int>(type: "integer", nullable: false),
                    ReportMonthOfYear = table.Column<int>(type: "integer", nullable: false),
                    ReportWeekOfYear = table.Column<int>(type: "integer", nullable: false),
                    ReportDayOfYear = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timesheets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "timetrack",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheets_TimeTrackUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheets_TimeTrackUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheets_TimeTrackUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CreatedById",
                schema: "timetrack",
                table: "Clients",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Name",
                schema: "timetrack",
                table: "Clients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_SalesManagerId",
                schema: "timetrack",
                table: "Clients",
                column: "SalesManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UpdatedById",
                schema: "timetrack",
                table: "Clients",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_CreatedAtUtc",
                schema: "timetrack",
                table: "OutboxEvents",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_WasSent",
                schema: "timetrack",
                table: "OutboxEvents",
                column: "WasSent");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                schema: "timetrack",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                schema: "timetrack",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ManagerUserId",
                schema: "timetrack",
                table: "Projects",
                column: "ManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                schema: "timetrack",
                table: "Projects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UpdatedById",
                schema: "timetrack",
                table: "Projects",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_CreatedById",
                schema: "timetrack",
                table: "ProjectUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectId",
                schema: "timetrack",
                table: "ProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_RefUserId",
                schema: "timetrack",
                table: "ProjectUsers",
                column: "RefUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UpdatedById",
                schema: "timetrack",
                table: "ProjectUsers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                schema: "timetrack",
                table: "ProjectUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_CreatedById",
                schema: "timetrack",
                table: "Timesheets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_ProjectId",
                schema: "timetrack",
                table: "Timesheets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_ReportYear_ReportDayOfYear",
                schema: "timetrack",
                table: "Timesheets",
                columns: ["ReportYear", "ReportDayOfYear"]);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_ReportYear_ReportMonthOfYear",
                schema: "timetrack",
                table: "Timesheets",
                columns: ["ReportYear", "ReportMonthOfYear"]);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_Unique_UserId_ProjectId_ReportForDate_TaskName",
                schema: "timetrack",
                table: "Timesheets",
                columns: ["UserId", "ProjectId", "ReportForDate", "TaskName"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_UpdatedById",
                schema: "timetrack",
                table: "Timesheets",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_UserId_ReportYear_ReportMonthOfYear",
                schema: "timetrack",
                table: "Timesheets",
                columns: ["UserId", "ReportYear", "ReportMonthOfYear"]);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_UserId_ReportYear_ReportMonthOfYear_ReportWeekOf~",
                schema: "timetrack",
                table: "Timesheets",
                columns: ["UserId", "ReportYear", "ReportMonthOfYear", "ReportWeekOfYear"]);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_UserId_ReportYear_ReportWeekOfYear",
                schema: "timetrack",
                table: "Timesheets",
                columns: ["UserId", "ReportYear", "ReportWeekOfYear"]);

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetSubmitStatuses_DecisionBy",
                schema: "timetrack",
                table: "TimesheetSubmitStatuses",
                column: "DecisionBy");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetSubmitStatuses_UserId_Year_Month",
                schema: "timetrack",
                table: "TimesheetSubmitStatuses",
                columns: ["UserId", "Year", "Month"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeTrackUsers_Email",
                schema: "timetrack",
                table: "TimeTrackUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTrackUsers_IsActive",
                schema: "timetrack",
                table: "TimeTrackUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTrackUsers_ResourceGroupId",
                schema: "timetrack",
                table: "TimeTrackUsers",
                column: "ResourceGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxEvents",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "ProjectUsers",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "ReportRequests",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "Timesheets",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "TimesheetSubmitStatuses",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "TimeTrackUsers",
                schema: "timetrack");

            migrationBuilder.DropTable(
                name: "TimeTrackResourceGroups",
                schema: "timetrack");
        }
    }
}
