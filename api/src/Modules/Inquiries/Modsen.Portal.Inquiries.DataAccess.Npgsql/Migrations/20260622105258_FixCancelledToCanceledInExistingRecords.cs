using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class FixCancelledToCanceledInExistingRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE inquiries.\"UserInquiries\" SET \"Status\" = 'Canceled' WHERE \"Status\" = 'Cancelled';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE inquiries.\"UserInquiries\" SET \"Status\" = 'Cancelled' WHERE \"Status\" = 'Canceled';");
        }
    }
}
