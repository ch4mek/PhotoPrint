using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Notification.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notification");

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Arguments = table.Column<string>(type: "json", maxLength: 2048, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    NotificationChannel = table.Column<string>(type: "text", nullable: false),
                    Sent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NotificationType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationUsers",
                schema: "notification",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    AvatarFileId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUsers", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationChannel_Sent_IsDeleted",
                schema: "notification",
                table: "Notifications",
                columns: ["NotificationChannel", "Sent", "IsDeleted"]);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUsers_Email",
                schema: "notification",
                table: "NotificationUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUsers_IsActive",
                schema: "notification",
                table: "NotificationUsers",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "NotificationUsers",
                schema: "notification");
        }
    }
}
