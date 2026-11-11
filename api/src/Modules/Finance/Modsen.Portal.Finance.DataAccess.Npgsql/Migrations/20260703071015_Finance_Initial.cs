using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Finance_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CurrencySymbol = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinanceClients",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LegalEntity = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SalesManagerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinanceDepartments",
                schema: "finance",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DepartmentOrderWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceDepartments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromCurrencyId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ToCurrencyId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CurrencyMultiplier = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(13,6)", precision: 13, scale: 6, nullable: false),
                    ExchangeDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalSchema: "finance",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalSchema: "finance",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    MonthOfYear = table.Column<int>(type: "integer", nullable: false),
                    ExpenseAmount = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalSchema: "finance",
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanceProjects",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceProjects_FinanceClients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "finance",
                        principalTable: "FinanceClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanceUsers",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "text", nullable: true),
                    LastNameRu = table.Column<string>(type: "text", nullable: true),
                    PatronymicRu = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceUsers_FinanceDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "finance",
                        principalTable: "FinanceDepartments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanceProjectUsers",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserOnProjectFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    UserOnProjectTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceProjectUsers_FinanceProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "finance",
                        principalTable: "FinanceProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinanceProjectUsers_FinanceUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "finance",
                        principalTable: "FinanceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRates",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<string>(type: "text", nullable: false),
                    CurrencyId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    RateFrom = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRates_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "finance",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRates_FinanceProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "finance",
                        principalTable: "FinanceProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRates_FinanceUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "finance",
                        principalTable: "FinanceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_FromCurrencyId",
                schema: "finance",
                table: "ExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ToCurrencyId",
                schema: "finance",
                table: "ExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_Name",
                schema: "finance",
                table: "ExpenseCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                schema: "finance",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceClients_Name",
                schema: "finance",
                table: "FinanceClients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceProjects_ClientId",
                schema: "finance",
                table: "FinanceProjects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceProjectUsers_ProjectId",
                schema: "finance",
                table: "FinanceProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceProjectUsers_UserId",
                schema: "finance",
                table: "FinanceProjectUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceUsers_DepartmentId",
                schema: "finance",
                table: "FinanceUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRates_CurrencyId",
                schema: "finance",
                table: "ProjectRates",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRates_ProjectId",
                schema: "finance",
                table: "ProjectRates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRates_UserId",
                schema: "finance",
                table: "ProjectRates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "Expenses",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "FinanceProjectUsers",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "ProjectRates",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "ExpenseCategories",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "FinanceProjects",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "FinanceUsers",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "FinanceClients",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "FinanceDepartments",
                schema: "finance");
        }
    }
}
