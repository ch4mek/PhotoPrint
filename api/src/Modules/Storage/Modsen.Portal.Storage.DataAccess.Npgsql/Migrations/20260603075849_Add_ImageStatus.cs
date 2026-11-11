using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Storage.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Add_ImageStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_applicationfiles_pending_backfill",
                schema: "storage",
                table: "ApplicationFiles");

            migrationBuilder.DropColumn(
                name: "IsImageVariantsBackfilled",
                schema: "storage",
                table: "ApplicationFiles");

            migrationBuilder.AddColumn<string>(
                name: "ImageStatus",
                schema: "storage",
                table: "ApplicationFiles",
                type: "text",
                nullable: false,
                defaultValue: "NotProcessed");

            migrationBuilder.CreateIndex(
                name: "ix_applicationfiles_pending_backfill",
                schema: "storage",
                table: "ApplicationFiles",
                column: "FileId",
                filter: "\"ImageStatus\" != 'Processed'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_applicationfiles_pending_backfill",
                schema: "storage",
                table: "ApplicationFiles");

            migrationBuilder.DropColumn(
                name: "ImageStatus",
                schema: "storage",
                table: "ApplicationFiles");

            migrationBuilder.AddColumn<bool>(
                name: "IsImageVariantsBackfilled",
                schema: "storage",
                table: "ApplicationFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_applicationfiles_pending_backfill",
                schema: "storage",
                table: "ApplicationFiles",
                column: "FileId",
                filter: "\"IsImageVariantsBackfilled\" = false");
        }
    }
}
