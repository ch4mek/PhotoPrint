using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Storage.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddFileSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileSource",
                schema: "storage",
                table: "ApplicationFiles",
                type: "text",
                nullable: false,
                defaultValue: "None");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSource",
                schema: "storage",
                table: "ApplicationFiles");
        }
    }
}
