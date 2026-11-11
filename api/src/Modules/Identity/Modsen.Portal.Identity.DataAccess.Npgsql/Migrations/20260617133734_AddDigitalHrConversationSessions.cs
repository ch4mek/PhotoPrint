using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalHrConversationSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DigitalHrConversationSession",
                schema: "identity",
                columns: table => new
                {
                    ConversationId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Watermark = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalHrConversationSession", x => x.ConversationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalHrConversationSession_UserId",
                schema: "identity",
                table: "DigitalHrConversationSession",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DigitalHrConversationSession",
                schema: "identity");
        }
    }
}
