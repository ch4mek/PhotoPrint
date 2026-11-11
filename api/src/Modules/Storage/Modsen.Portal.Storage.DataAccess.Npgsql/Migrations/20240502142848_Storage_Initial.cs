using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Storage.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Storage_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "storage");

            migrationBuilder.CreateTable(
                name: "ApplicationFiles",
                schema: "storage",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BucketName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VirtualDirectory = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UploadedByUserId = table.Column<int>(type: "integer", nullable: true),
                    AvailableTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsImageVariantsBackfilled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ImageVariantsBackfilledAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFiles", x => x.FileId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                schema: "storage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JsonData = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    WasSent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_applicationfiles_pending_backfill",
                schema: "storage",
                table: "ApplicationFiles",
                column: "FileId",
                filter: "\"IsImageVariantsBackfilled\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_CreatedAtUtc",
                schema: "storage",
                table: "OutboxEvents",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_WasSent",
                schema: "storage",
                table: "OutboxEvents",
                column: "WasSent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationFiles",
                schema: "storage");

            migrationBuilder.DropTable(
                name: "OutboxEvents",
                schema: "storage");
        }
    }
}
