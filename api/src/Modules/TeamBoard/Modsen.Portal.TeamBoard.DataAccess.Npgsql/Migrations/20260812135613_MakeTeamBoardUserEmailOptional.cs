using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class MakeTeamBoardUserEmailOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "teamboard",
                table: "TeamBoardUsers",
                type: "character varying(550)",
                maxLength: 550,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(550)",
                oldMaxLength: 550);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "teamboard",
                table: "TeamBoardUsers",
                type: "character varying(550)",
                maxLength: 550,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(550)",
                oldMaxLength: 550,
                oldNullable: true);
        }
    }
}
