using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Storage.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePurpose",
                schema: "storage",
                table: "ApplicationFiles",
                type: "text",
                nullable: false,
                defaultValue: "General");

            migrationBuilder.Sql("""
                update storage."ApplicationFiles"
                set "FilePurpose" = 'UserAvatar'
                where "FileId" in (
                    select "AvatarFileId"
                    from identity."Users"
                    where "AvatarFileId" is not null
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePurpose",
                schema: "storage",
                table: "ApplicationFiles");
        }
    }
}
