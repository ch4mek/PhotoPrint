using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserInternshipToInternshipDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInternships",
                schema: "teamboard");

            migrationBuilder.CreateTable(
                name: "InternshipDetails",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Specialization = table.Column<string>(type: "text", nullable: false),
                    ApprovedPoints = table.Column<int>(type: "integer", nullable: true),
                    CompletedPoints = table.Column<int>(type: "integer", nullable: true),
                    EngineerStage = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PlannedEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    TypeOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"Type\" WHEN 'Primary' THEN 100 WHEN 'Extended' THEN 200 WHEN 'NDA' THEN 300 ELSE 400 END", stored: true),
                    FinalResult = table.Column<string>(type: "text", nullable: true),
                    FinalResultOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"FinalResult\" WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 WHEN 'NotAvailable' THEN 300 ELSE 400 END", stored: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternshipDetails_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipDetails_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipDetails_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternshipDetails_CreatedById",
                schema: "teamboard",
                table: "InternshipDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipDetails_UpdatedById",
                schema: "teamboard",
                table: "InternshipDetails",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipDetails_UserId",
                schema: "teamboard",
                table: "InternshipDetails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternshipDetails",
                schema: "teamboard");

            migrationBuilder.CreateTable(
                name: "UserInternships",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedPoints = table.Column<int>(type: "integer", nullable: true),
                    CompletedPoints = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EngineerStage = table.Column<int>(type: "integer", nullable: false),
                    FinalResult = table.Column<string>(type: "text", nullable: true),
                    FinalResultOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"FinalResult\" WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 WHEN 'NotAvailable' THEN 300 ELSE 400 END", stored: true),
                    PlannedEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Specialization = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    TypeOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"Type\" WHEN 'Primary' THEN 100 WHEN 'Extended' THEN 200 WHEN 'NDA' THEN 300 ELSE 400 END", stored: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInternships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInternships_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInternships_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInternships_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInternships_CreatedById",
                schema: "teamboard",
                table: "UserInternships",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInternships_UpdatedById",
                schema: "teamboard",
                table: "UserInternships",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInternships_UserId",
                schema: "teamboard",
                table: "UserInternships",
                column: "UserId");
        }
    }
}
