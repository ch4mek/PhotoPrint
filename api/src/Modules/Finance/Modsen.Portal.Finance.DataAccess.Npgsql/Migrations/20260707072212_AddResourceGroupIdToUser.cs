using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceGroupIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResourceGroupId",
                schema: "finance",
                table: "FinanceUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceGroupId",
                schema: "finance",
                table: "FinanceUsers");
        }
    }
}
