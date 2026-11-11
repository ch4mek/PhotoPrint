using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "attendance");

            migrationBuilder.CreateTable(
                name: "ResourceGroups",
                schema: "attendance",
                columns: table => new
                {
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroups", x => x.ResourceGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "attendance",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PatronymicRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    Position = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_ResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalSchema: "attendance",
                        principalTable: "ResourceGroups",
                        principalColumn: "ResourceGroupId");
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                schema: "attendance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CheckedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Door = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "attendance",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UserId",
                schema: "attendance",
                table: "Attendances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResourceGroupId",
                schema: "attendance",
                table: "Users",
                column: "ResourceGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances",
                schema: "attendance");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "attendance");

            migrationBuilder.DropTable(
                name: "ResourceGroups",
                schema: "attendance");
        }
    }
}
