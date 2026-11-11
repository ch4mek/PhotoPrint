using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class IncreaceUserPerformanceReviewColumnsTo5000Characters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResponsibleUserFeedback",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeFeedback",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Agreements",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResponsibleUserFeedback",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeFeedback",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Agreements",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);
        }
    }
}
