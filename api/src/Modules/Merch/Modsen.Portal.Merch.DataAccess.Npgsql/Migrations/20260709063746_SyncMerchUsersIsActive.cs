using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class SyncMerchUsersIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE merch.\"MerchUsers\" mu SET \"IsActive\" = u.\"IsActive\" FROM identity.\"Users\" u WHERE mu.\"UserId\" = u.\"Id\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
