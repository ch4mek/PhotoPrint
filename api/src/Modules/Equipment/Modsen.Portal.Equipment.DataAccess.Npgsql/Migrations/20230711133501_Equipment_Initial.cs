using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Equipment_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "equipment");

            migrationBuilder.CreateTable(
                name: "EquipmentDepartments",
                schema: "equipment",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDepartments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentReports",
                schema: "equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerialNumber = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Data = table.Column<string>(type: "json", maxLength: 1048576, nullable: false),
                    DataHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    AssembledAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentUsers",
                schema: "equipment",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_EquipmentUsers_EquipmentUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentUsers_EquipmentUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentReportRelevancePeriods",
                schema: "equipment",
                columns: table => new
                {
                    FromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EquipmentReportId = table.Column<int>(type: "integer", nullable: false),
                    ToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentReportRelevancePeriods", x => new { x.EquipmentReportId, x.FromUtc });
                    table.ForeignKey(
                        name: "FK_EquipmentReportRelevancePeriods_EquipmentReports_EquipmentR~",
                        column: x => x.EquipmentReportId,
                        principalSchema: "equipment",
                        principalTable: "EquipmentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                schema: "equipment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentTypes_EquipmentUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentTypes_EquipmentUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                schema: "equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    BrokenStatus = table.Column<string>(type: "text", nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    InventoryNumber = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    PurchaseCurrency = table.Column<string>(type: "text", nullable: false),
                    PurchasePriceUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PurchasePlace = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GuaranteeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Characteristics = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    JiraTask = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ApproverId = table.Column<int>(type: "integer", nullable: true),
                    TypeId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Storage = table.Column<string>(type: "text", nullable: true),
                    RepairPlace = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "equipment",
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentAssigns",
                schema: "equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: false),
                    ApproverId = table.Column<int>(type: "integer", nullable: true),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAssigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentAssigns_EquipmentUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssigns_EquipmentUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssigns_EquipmentUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssigns_EquipmentUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "equipment",
                        principalTable: "EquipmentUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_EquipmentAssigns_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "equipment",
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssigns_ApproverId",
                schema: "equipment",
                table: "EquipmentAssigns",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssigns_AssignedToUserId",
                schema: "equipment",
                table: "EquipmentAssigns",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssigns_CreatedById",
                schema: "equipment",
                table: "EquipmentAssigns",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssigns_EquipmentId",
                schema: "equipment",
                table: "EquipmentAssigns",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssigns_UpdatedById",
                schema: "equipment",
                table: "EquipmentAssigns",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ApproverId",
                schema: "equipment",
                table: "Equipments",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_CreatedById",
                schema: "equipment",
                table: "Equipments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_InventoryNumber",
                schema: "equipment",
                table: "Equipments",
                column: "InventoryNumber",
                unique: true,
                filter: "\"InventoryNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                schema: "equipment",
                table: "Equipments",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TypeId",
                schema: "equipment",
                table: "Equipments",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_UpdatedById",
                schema: "equipment",
                table: "Equipments",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_CreatedById",
                schema: "equipment",
                table: "EquipmentTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_UpdatedById",
                schema: "equipment",
                table: "EquipmentTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentUsers_CreatedById",
                schema: "equipment",
                table: "EquipmentUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentUsers_Email",
                schema: "equipment",
                table: "EquipmentUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentUsers_IsActive",
                schema: "equipment",
                table: "EquipmentUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentUsers_UpdatedById",
                schema: "equipment",
                table: "EquipmentUsers",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentAssigns",
                schema: "equipment");

            migrationBuilder.DropTable(
                name: "EquipmentDepartments",
                schema: "equipment");

            migrationBuilder.DropTable(
                name: "EquipmentReportRelevancePeriods",
                schema: "equipment");

            migrationBuilder.DropTable(
                name: "Equipments",
                schema: "equipment");

            migrationBuilder.DropTable(
                name: "EquipmentReports",
                schema: "equipment");

            migrationBuilder.DropTable(
                name: "EquipmentTypes",
                schema: "equipment");

            migrationBuilder.DropTable(
                name: "EquipmentUsers",
                schema: "equipment");
        }
    }
}
