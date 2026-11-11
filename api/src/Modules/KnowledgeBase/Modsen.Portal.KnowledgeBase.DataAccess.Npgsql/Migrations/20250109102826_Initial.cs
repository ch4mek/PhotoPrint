using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "knowledgebase");

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBaseUsers",
                schema: "knowledgebase",
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
                    table.PrimaryKey("PK_KnowledgeBaseUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCategories",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountCategories_KnowledgeBaseUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscountCategories_KnowledgeBaseUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentGroupId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Icon = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailableOnMainScreen = table.Column<bool>(type: "boolean", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Groups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalSchema: "knowledgebase",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_KnowledgeBaseUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_KnowledgeBaseUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCategoryTags",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountCategoryId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCategoryTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountCategoryTags_DiscountCategories_DiscountCategoryId",
                        column: x => x.DiscountCategoryId,
                        principalSchema: "knowledgebase",
                        principalTable: "DiscountCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscountCategoryTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "knowledgebase",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ImageFileId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DiscountPercent = table.Column<int>(type: "integer", nullable: false),
                    DiscountPercentTo = table.Column<int>(type: "integer", nullable: true),
                    CompanyType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Location = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_DiscountCategories_DiscountCategoryId",
                        column: x => x.DiscountCategoryId,
                        principalSchema: "knowledgebase",
                        principalTable: "DiscountCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discounts_KnowledgeBaseUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discounts_KnowledgeBaseUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Icon = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Keywords = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailableOnMainScreen = table.Column<bool>(type: "boolean", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "knowledgebase",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_KnowledgeBaseUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_KnowledgeBaseUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "knowledgebase",
                        principalTable: "KnowledgeBaseUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCities",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountCities_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "knowledgebase",
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscountCities_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalSchema: "knowledgebase",
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountReferences",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    SocialMediaUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountReferences_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalSchema: "knowledgebase",
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountTags",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountTags_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalSchema: "knowledgebase",
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscountTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "knowledgebase",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleReferences",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    InternalFileId = table.Column<string>(type: "text", nullable: true),
                    ExternalUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleReferences_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "knowledgebase",
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTags",
                schema: "knowledgebase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleTags_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "knowledgebase",
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticleTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "knowledgebase",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleReferences_ArticleId",
                schema: "knowledgebase",
                table: "ArticleReferences",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedById",
                schema: "knowledgebase",
                table: "Articles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_GroupId",
                schema: "knowledgebase",
                table: "Articles",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UpdatedById",
                schema: "knowledgebase",
                table: "Articles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_ArticleId",
                schema: "knowledgebase",
                table: "ArticleTags",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_TagId",
                schema: "knowledgebase",
                table: "ArticleTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                schema: "knowledgebase",
                table: "Cities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCategories_CreatedById",
                schema: "knowledgebase",
                table: "DiscountCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCategories_UpdatedById",
                schema: "knowledgebase",
                table: "DiscountCategories",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCategoryTags_DiscountCategoryId",
                schema: "knowledgebase",
                table: "DiscountCategoryTags",
                column: "DiscountCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCategoryTags_TagId",
                schema: "knowledgebase",
                table: "DiscountCategoryTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCities_CityId",
                schema: "knowledgebase",
                table: "DiscountCities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCities_DiscountId",
                schema: "knowledgebase",
                table: "DiscountCities",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountReferences_DiscountId",
                schema: "knowledgebase",
                table: "DiscountReferences",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CreatedById",
                schema: "knowledgebase",
                table: "Discounts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_DiscountCategoryId",
                schema: "knowledgebase",
                table: "Discounts",
                column: "DiscountCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_UpdatedById",
                schema: "knowledgebase",
                table: "Discounts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTags_DiscountId",
                schema: "knowledgebase",
                table: "DiscountTags",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTags_TagId",
                schema: "knowledgebase",
                table: "DiscountTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatedById",
                schema: "knowledgebase",
                table: "Groups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupId",
                schema: "knowledgebase",
                table: "Groups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_UpdatedById",
                schema: "knowledgebase",
                table: "Groups",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                schema: "knowledgebase",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleReferences",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "ArticleTags",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "DiscountCategoryTags",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "DiscountCities",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "DiscountReferences",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "DiscountTags",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "Articles",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "Discounts",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "DiscountCategories",
                schema: "knowledgebase");

            migrationBuilder.DropTable(
                name: "KnowledgeBaseUsers",
                schema: "knowledgebase");
        }
    }
}
