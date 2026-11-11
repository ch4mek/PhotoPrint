using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class FixCancelledToCanceled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusOrderWeight",
                schema: "inquiries",
                table: "UserInquiries",
                type: "integer",
                nullable: false,
                computedColumnSql: "CASE \"Status\" WHEN 'Approved' THEN 0 WHEN 'Canceled' THEN 50 WHEN 'Rejected' THEN 100 WHEN 'Open' THEN 200 ELSE 200 END",
                stored: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComputedColumnSql: "CASE \"Status\" WHEN 'Approved' THEN 0 WHEN 'Cancelled' THEN 50 WHEN 'Rejected' THEN 100 WHEN 'Open' THEN 200 ELSE 200 END",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusOrderWeight",
                schema: "inquiries",
                table: "UserInquiries",
                type: "integer",
                nullable: false,
                computedColumnSql: "CASE \"Status\" WHEN 'Approved' THEN 0 WHEN 'Cancelled' THEN 50 WHEN 'Rejected' THEN 100 WHEN 'Open' THEN 200 ELSE 200 END",
                stored: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComputedColumnSql: "CASE \"Status\" WHEN 'Approved' THEN 0 WHEN 'Canceled' THEN 50 WHEN 'Rejected' THEN 100 WHEN 'Open' THEN 200 ELSE 200 END",
                oldStored: true);
        }
    }
}
