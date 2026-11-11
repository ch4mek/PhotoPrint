using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseCategoryNamePartialUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_Name",
                schema: "finance",
                table: "ExpenseCategories");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_Name",
                schema: "finance",
                table: "ExpenseCategories",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_Name",
                schema: "finance",
                table: "ExpenseCategories");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_Name",
                schema: "finance",
                table: "ExpenseCategories",
                column: "Name",
                unique: true);
        }
    }
}
