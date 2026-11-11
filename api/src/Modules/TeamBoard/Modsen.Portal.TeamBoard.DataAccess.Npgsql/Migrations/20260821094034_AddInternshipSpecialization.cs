using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddInternshipSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                schema: "teamboard",
                table: "TeamBoardUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NdaEndDate",
                schema: "teamboard",
                table: "TeamBoardUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NdaStartDate",
                schema: "teamboard",
                table: "TeamBoardUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FinalResultOrderWeight",
                schema: "teamboard",
                table: "InternshipDetails",
                type: "integer",
                nullable: false,
                computedColumnSql: "CASE \"FinalResult\" WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 ELSE 300 END",
                stored: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComputedColumnSql: "CASE \"FinalResult\" WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 WHEN 'NotAvailable' THEN 300 ELSE 400 END",
                oldStored: true);

            migrationBuilder.CreateTable(
                name: "InternshipSpecializations",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternshipDetailId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationTemplateId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializations_InternshipDetails_InternshipDetai~",
                        column: x => x.InternshipDetailId,
                        principalSchema: "teamboard",
                        principalTable: "InternshipDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializations_SeniorityLevels_SeniorityLevelId",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializations_SpecializationTemplates_Specializ~",
                        column: x => x.SpecializationTemplateId,
                        principalSchema: "teamboard",
                        principalTable: "SpecializationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializations_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InternshipSpecializationHardSkills",
                schema: "teamboard",
                columns: table => new
                {
                    InternsipSpecializationId = table.Column<int>(type: "integer", nullable: false),
                    HardSkillId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipSpecializationHardSkills", x => new { x.InternsipSpecializationId, x.HardSkillId });
                    table.ForeignKey(
                        name: "FK_InternshipSpecializationHardSkills_HardSkills_HardSkillId",
                        column: x => x.HardSkillId,
                        principalSchema: "teamboard",
                        principalTable: "HardSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializationHardSkills_InternshipSpecialization~",
                        column: x => x.InternsipSpecializationId,
                        principalSchema: "teamboard",
                        principalTable: "InternshipSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializationHardSkills_SeniorityLevels_Seniorit~",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializationHardSkills_TeamBoardUsers_CreatedBy~",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSpecializationHardSkills_TeamBoardUsers_UpdatedBy~",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializationHardSkills_CreatedById",
                schema: "teamboard",
                table: "InternshipSpecializationHardSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializationHardSkills_HardSkillId",
                schema: "teamboard",
                table: "InternshipSpecializationHardSkills",
                column: "HardSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializationHardSkills_SeniorityLevelId",
                schema: "teamboard",
                table: "InternshipSpecializationHardSkills",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializationHardSkills_UpdatedById",
                schema: "teamboard",
                table: "InternshipSpecializationHardSkills",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializations_CreatedById",
                schema: "teamboard",
                table: "InternshipSpecializations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializations_InternshipDetailId_Specialization~",
                schema: "teamboard",
                table: "InternshipSpecializations",
                columns: new[] { "InternshipDetailId", "SpecializationTemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializations_SeniorityLevelId",
                schema: "teamboard",
                table: "InternshipSpecializations",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSpecializations_SpecializationTemplateId",
                schema: "teamboard",
                table: "InternshipSpecializations",
                column: "SpecializationTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternshipSpecializationHardSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "InternshipSpecializations",
                schema: "teamboard");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "teamboard",
                table: "TeamBoardUsers");

            migrationBuilder.DropColumn(
                name: "NdaEndDate",
                schema: "teamboard",
                table: "TeamBoardUsers");

            migrationBuilder.DropColumn(
                name: "NdaStartDate",
                schema: "teamboard",
                table: "TeamBoardUsers");

            migrationBuilder.AlterColumn<int>(
                name: "FinalResultOrderWeight",
                schema: "teamboard",
                table: "InternshipDetails",
                type: "integer",
                nullable: false,
                computedColumnSql: "CASE \"FinalResult\" WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 WHEN 'NotAvailable' THEN 300 ELSE 400 END",
                stored: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComputedColumnSql: "CASE \"FinalResult\" WHEN 'Success' THEN 100 WHEN 'Fired' THEN 200 ELSE 300 END",
                oldStored: true);
        }
    }
}
