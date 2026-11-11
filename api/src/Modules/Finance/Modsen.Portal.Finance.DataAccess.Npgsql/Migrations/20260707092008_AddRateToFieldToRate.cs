using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddRateToFieldToRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "RateTo",
                schema: "finance",
                table: "ProjectRates",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RateTo",
                schema: "finance",
                table: "ProjectRates");
        }
    }
}
