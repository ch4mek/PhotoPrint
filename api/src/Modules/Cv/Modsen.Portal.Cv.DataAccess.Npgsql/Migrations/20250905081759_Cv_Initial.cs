using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Cv.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Cv_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cv");

            migrationBuilder.CreateTable(
                name: "CvClients",
                schema: "cv",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvClients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "CvProjects",
                schema: "cv",
                columns: table => new
                {
                    ProjectId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvProjects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "CvUsers",
                schema: "cv",
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
                    Position = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Location = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Cvs",
                schema: "cv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CvName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StatusOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"Status\" WHEN 'Sent' THEN 100 WHEN 'Draft' THEN 0 ELSE 0 END", stored: true),
                    IsVisibleToUser = table.Column<bool>(type: "boolean", nullable: false),
                    UserFirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UserLastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UserProjectPosition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UserLocation = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UserEducation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UserSkills = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UserAbout = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsCorporateDesign = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cvs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cvs_CvClients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cv",
                        principalTable: "CvClients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cvs_CvProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "cv",
                        principalTable: "CvProjects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cvs_CvUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "cv",
                        principalTable: "CvUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cvs_CvUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "cv",
                        principalTable: "CvUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cvs_CvUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "cv",
                        principalTable: "CvUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "cv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    CvId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Languages_Cvs_CvId",
                        column: x => x.CvId,
                        principalSchema: "cv",
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectExperiences",
                schema: "cv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Position = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TeamSize = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Responsibilities = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Environment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CvId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectExperiences_Cvs_CvId",
                        column: x => x.CvId,
                        principalSchema: "cv",
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillGroups",
                schema: "cv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CvId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillGroups_Cvs_CvId",
                        column: x => x.CvId,
                        principalSchema: "cv",
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkExperiences",
                schema: "cv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Specialization = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ExperienceInYears = table.Column<int>(type: "integer", nullable: false),
                    CvId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkExperiences_Cvs_CvId",
                        column: x => x.CvId,
                        principalSchema: "cv",
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "cv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
                    ExperienceInYears = table.Column<int>(type: "integer", nullable: true),
                    LastUsedYear = table.Column<int>(type: "integer", nullable: true),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    SkillGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_SkillGroups_SkillGroupId",
                        column: x => x.SkillGroupId,
                        principalSchema: "cv",
                        principalTable: "SkillGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_ClientId",
                schema: "cv",
                table: "Cvs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_CreatedById",
                schema: "cv",
                table: "Cvs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_ProjectId",
                schema: "cv",
                table: "Cvs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_UpdatedById",
                schema: "cv",
                table: "Cvs",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Cvs_UserId",
                schema: "cv",
                table: "Cvs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CvId",
                schema: "cv",
                table: "Languages",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExperiences_CvId",
                schema: "cv",
                table: "ProjectExperiences",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillGroups_CvId",
                schema: "cv",
                table: "SkillGroups",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillGroupId",
                schema: "cv",
                table: "Skills",
                column: "SkillGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperiences_CvId",
                schema: "cv",
                table: "WorkExperiences",
                column: "CvId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "ProjectExperiences",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "WorkExperiences",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "SkillGroups",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "Cvs",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "CvClients",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "CvProjects",
                schema: "cv");

            migrationBuilder.DropTable(
                name: "CvUsers",
                schema: "cv");
        }
    }
}
