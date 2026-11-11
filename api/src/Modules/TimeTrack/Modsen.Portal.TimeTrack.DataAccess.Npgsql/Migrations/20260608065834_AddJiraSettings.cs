using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddJiraSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JiraSettings",
                schema: "timetrack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Pat = table.Column<string>(type: "text", nullable: false),
                    JiraProjectTag = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JiraSettings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "timetrack",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JiraSettings_TimeTrackUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "timetrack",
                        principalTable: "TimeTrackUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JiraSettings_ProjectId",
                schema: "timetrack",
                table: "JiraSettings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_JiraSettings_UserId",
                schema: "timetrack",
                table: "JiraSettings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JiraSettings",
                schema: "timetrack");
        }
    }
}
