using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToProjectRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "finance",
                table: "ProjectRates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "finance",
                table: "ProjectRates");
        }
    }
}
