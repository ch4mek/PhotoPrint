using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Merch_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "merch");

            migrationBuilder.CreateTable(
                name: "MerchSizes",
                schema: "merch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    IsForClothing = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchUsers",
                schema: "merch",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                schema: "merch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Location = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                schema: "merch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsClothing = table.Column<string>(type: "character varying(1)", nullable: false),
                    Supplier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    PhotoFileId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ArchivedById = table.Column<int>(type: "integer", nullable: true),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_MerchUsers_ArchivedById",
                        column: x => x.ArchivedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_MerchUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_MerchUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemTransactionGroups",
                schema: "merch",
                columns: table => new
                {
                    GroupCode = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTransactionGroups", x => x.GroupCode);
                    table.ForeignKey(
                        name: "FK_ItemTransactionGroups_MerchUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kits",
                schema: "merch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    PhotoFileId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Available"),
                    ArchivedById = table.Column<int>(type: "integer", nullable: true),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kits_MerchUsers_ArchivedById",
                        column: x => x.ArchivedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kits_MerchUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kits_MerchUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemSizeMinAmounts",
                schema: "merch",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    SizeId = table.Column<string>(type: "character varying(3)", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    MinAmount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSizeMinAmounts", x => new { x.ItemId, x.SizeId, x.WarehouseId });
                    table.ForeignKey(
                        name: "FK_ItemSizeMinAmounts_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "merch",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSizeMinAmounts_MerchSizes_SizeId",
                        column: x => x.SizeId,
                        principalSchema: "merch",
                        principalTable: "MerchSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSizeMinAmounts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "merch",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemTransactions",
                schema: "merch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"Type\" WHEN 'Transfer' THEN 200 WHEN 'Receipt' THEN 100 WHEN 'WriteOff' THEN 0 ELSE 0 END", stored: true),
                    GroupCode = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    SizeId = table.Column<string>(type: "character varying(3)", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: true),
                    UserRecipientId = table.Column<int>(type: "integer", nullable: true),
                    WarehouseRecipientId = table.Column<int>(type: "integer", nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_ItemTransactionGroups_GroupCode",
                        column: x => x.GroupCode,
                        principalSchema: "merch",
                        principalTable: "ItemTransactionGroups",
                        principalColumn: "GroupCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "merch",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_MerchSizes_SizeId",
                        column: x => x.SizeId,
                        principalSchema: "merch",
                        principalTable: "MerchSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_MerchUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_MerchUsers_UserRecipientId",
                        column: x => x.UserRecipientId,
                        principalSchema: "merch",
                        principalTable: "MerchUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ItemTransactions_Warehouses_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "merch",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "merch",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_Warehouses_WarehouseRecipientId",
                        column: x => x.WarehouseRecipientId,
                        principalSchema: "merch",
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KitItems",
                schema: "merch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KitId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "merch",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KitItems_Kits_KitId",
                        column: x => x.KitId,
                        principalSchema: "merch",
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitMinAmounts",
                schema: "merch",
                columns: table => new
                {
                    KitId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    MinAmount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitMinAmounts", x => new { x.KitId, x.WarehouseId });
                    table.ForeignKey(
                        name: "FK_KitMinAmounts_Kits_KitId",
                        column: x => x.KitId,
                        principalSchema: "merch",
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KitMinAmounts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "merch",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ArchivedById",
                schema: "merch",
                table: "Items",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedById",
                schema: "merch",
                table: "Items",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedById",
                schema: "merch",
                table: "Items",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSizeMinAmounts_SizeId",
                schema: "merch",
                table: "ItemSizeMinAmounts",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSizeMinAmounts_WarehouseId",
                schema: "merch",
                table: "ItemSizeMinAmounts",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactionGroups_CreatedById",
                schema: "merch",
                table: "ItemTransactionGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_CreatedById",
                schema: "merch",
                table: "ItemTransactions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_GroupCode",
                schema: "merch",
                table: "ItemTransactions",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_ItemId",
                schema: "merch",
                table: "ItemTransactions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_SenderId",
                schema: "merch",
                table: "ItemTransactions",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_SizeId",
                schema: "merch",
                table: "ItemTransactions",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_UserRecipientId",
                schema: "merch",
                table: "ItemTransactions",
                column: "UserRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_WarehouseId",
                schema: "merch",
                table: "ItemTransactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_WarehouseRecipientId",
                schema: "merch",
                table: "ItemTransactions",
                column: "WarehouseRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_KitItems_ItemId",
                schema: "merch",
                table: "KitItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KitItems_KitId",
                schema: "merch",
                table: "KitItems",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_KitMinAmounts_WarehouseId",
                schema: "merch",
                table: "KitMinAmounts",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Kits_ArchivedById",
                schema: "merch",
                table: "Kits",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Kits_CreatedById",
                schema: "merch",
                table: "Kits",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Kits_UpdatedById",
                schema: "merch",
                table: "Kits",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSizeMinAmounts",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "ItemTransactions",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "KitItems",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "KitMinAmounts",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "ItemTransactionGroups",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "MerchSizes",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "Items",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "Kits",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "Warehouses",
                schema: "merch");

            migrationBuilder.DropTable(
                name: "MerchUsers",
                schema: "merch");
        }
    }
}
