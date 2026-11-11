#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Modsen.Portal.Migrator.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Migrator_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "migration");

            migrationBuilder.Sql("DROP TABLE IF EXISTS migration.\"MigrationTask\"");

            migrationBuilder.CreateTable(
                name: "MigrationTask",
                schema: "migration",
                columns: table => new
                {
                    TaskName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationTask", x => x.TaskName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MigrationTask",
                schema: "migration");
        }
    }
}
