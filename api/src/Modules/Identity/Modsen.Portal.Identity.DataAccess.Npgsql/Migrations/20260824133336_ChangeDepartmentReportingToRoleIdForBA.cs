using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDepartmentReportingToRoleIdForBA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE identity."Departments"
                SET "DepartmentReportingToRoleId" = (
                    SELECT "Id"
                    FROM identity."AspNetRoles"
                    WHERE "Name" = 'CTO'
                )
                WHERE "Id" = 'BA';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE identity."Departments"
                SET "DepartmentReportingToRoleId" = (
                    SELECT "Id"
                    FROM identity."AspNetRoles"
                    WHERE "Name" = 'CEO'
                )
                WHERE "Id" = 'BA';
                """);
        }
    }
}
