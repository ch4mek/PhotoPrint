using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddIsTimeOnProjectBillableToFinanceProjectUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTimeOnProjectBillable",
                schema: "finance",
                table: "FinanceProjectUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE finance."FinanceProjectUsers" fpu
                SET "IsTimeOnProjectBillable" = pu."IsTimeOnProjectBillable"
                FROM timetrack."ProjectUsers" pu
                WHERE fpu."Id" = pu."Id";
             """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTimeOnProjectBillable",
                schema: "finance",
                table: "FinanceProjectUsers");
        }
    }
}
