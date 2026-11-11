using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByIdToTimeTrackUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                schema: "timetrack",
                table: "TimeTrackUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "timetrack",
                table: "TimeTrackUsers");
        }
    }
}
