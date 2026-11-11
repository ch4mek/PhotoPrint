using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalEmailAttributeToJiraSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "timetrack",
                table: "JiraSettings",
                type: "character varying(550)",
                maxLength: 550,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "timetrack",
                table: "JiraSettings");
        }
    }
}
