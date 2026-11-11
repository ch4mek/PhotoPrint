using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddLegalEntityToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LegalEntity",
                schema: "timetrack",
                table: "Clients",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegalEntity",
                schema: "timetrack",
                table: "Clients");
        }
    }
}
