using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInterviewStatusOrderWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusOrderWeight",
                schema: "teamboard",
                table: "UserInterviews",
                type: "integer",
                nullable: false,
                computedColumnSql: "CASE \"Status\" WHEN 'WaitingForInterview' THEN 100 WHEN 'WaitingForResult' THEN 200 WHEN 'Failed' THEN 300 WHEN 'Success' THEN 400 WHEN 'Started' THEN 500 WHEN 'NotStarted' THEN 600 WHEN 'Canceled' THEN 700 WHEN 'NoFeedback' THEN 800 WHEN 'NoNextStepScheduled' THEN 900 ELSE 1000 END",
                stored: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComputedColumnSql: "CASE \"Status\" WHEN 'WaitingForInterview' THEN 100 WHEN 'WaitingForResult' THEN 200 WHEN 'Failed' THEN 300 WHEN 'Success' THEN 400 WHEN 'Started' THEN 500 WHEN 'NotStarted' THEN 600 WHEN 'NoFeedback' THEN 700 WHEN 'NoNextStepScheduled' THEN 800 ELSE 900 END",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusOrderWeight",
                schema: "teamboard",
                table: "UserInterviews",
                type: "integer",
                nullable: false,
                computedColumnSql: "CASE \"Status\" WHEN 'WaitingForInterview' THEN 100 WHEN 'WaitingForResult' THEN 200 WHEN 'Failed' THEN 300 WHEN 'Success' THEN 400 WHEN 'Started' THEN 500 WHEN 'NotStarted' THEN 600 WHEN 'NoFeedback' THEN 700 WHEN 'NoNextStepScheduled' THEN 800 ELSE 900 END",
                stored: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComputedColumnSql: "CASE \"Status\" WHEN 'WaitingForInterview' THEN 100 WHEN 'WaitingForResult' THEN 200 WHEN 'Failed' THEN 300 WHEN 'Success' THEN 400 WHEN 'Started' THEN 500 WHEN 'NotStarted' THEN 600 WHEN 'Canceled' THEN 700 WHEN 'NoFeedback' THEN 800 WHEN 'NoNextStepScheduled' THEN 900 ELSE 1000 END",
                oldStored: true);
        }
    }
}
