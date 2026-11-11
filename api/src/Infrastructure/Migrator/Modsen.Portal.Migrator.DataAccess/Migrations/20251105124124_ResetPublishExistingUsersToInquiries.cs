using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Migrator.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ResetPublishExistingUsersToInquiries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 delete from migration."MigrationTask"
                                 where "TaskName" = 'PublishExistingUsersToInquiries'
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
