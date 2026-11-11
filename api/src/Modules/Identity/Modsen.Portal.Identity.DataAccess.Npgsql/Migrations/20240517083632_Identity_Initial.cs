using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Identity_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    IsCompanyManager = table.Column<bool>(type: "boolean", nullable: false),
                    PublicName = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimGroups",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomDetails",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferenceId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessageSource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MicrofrontendsConfigs",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JsonData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrofrontendsConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                schema: "identity",
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

            migrationBuilder.CreateTable(
                name: "PermissionDefinitions",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalSettings",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ValueType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedirectRules",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RedirectKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RedirectTo = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedirectRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ModsenApiValue = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    DepartmentReportingToRoleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_AspNetRoles_DepartmentReportingToRoleId",
                        column: x => x.DepartmentReportingToRoleId,
                        principalSchema: "identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                schema: "identity",
                columns: table => new
                {
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AllowedValueType = table.Column<string>(type: "text", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimType);
                    table.ForeignKey(
                        name: "FK_Claims_ClaimGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "identity",
                        principalTable: "ClaimGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceGroups",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceGroups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "identity",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentClaims_Claims_ClaimType",
                        column: x => x.ClaimType,
                        principalSchema: "identity",
                        principalTable: "Claims",
                        principalColumn: "ClaimType",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentClaims_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "identity",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Claims_ClaimType",
                        column: x => x.ClaimType,
                        principalSchema: "identity",
                        principalTable: "Claims",
                        principalColumn: "ClaimType",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PatronymicRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    AvatarColor = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    AvatarFileId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SignatureFullName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SignaturePosition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SignaturePhone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    RegisteredLocationId = table.Column<int>(type: "integer", nullable: false),
                    CurrentLocation = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "identity",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_ResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalSchema: "identity",
                        principalTable: "ResourceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorPicker",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorPicker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorPicker_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionAssignments",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PermissionDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ResourceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ResourceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionAssignments_PermissionDefinitions_PermissionDefin~",
                        column: x => x.PermissionDefinitionId,
                        principalSchema: "identity",
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Claims_ClaimType",
                        column: x => x.ClaimType,
                        principalSchema: "identity",
                        principalTable: "Claims",
                        principalColumn: "ClaimType",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDepartmentHistory",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDepartmentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDepartmentHistory_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "identity",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDepartmentHistory_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessages_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "identity",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "identity",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_GroupId",
                schema: "identity",
                table: "Claims",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorPicker_UserId_Color",
                schema: "identity",
                table: "ColorPicker",
                columns: ["UserId", "Color"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomDetails_ReferenceType_ReferenceId",
                schema: "identity",
                table: "CustomDetails",
                columns: ["ReferenceType", "ReferenceId"]);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentClaims_ClaimType",
                schema: "identity",
                table: "DepartmentClaims",
                column: "ClaimType");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentClaims_DepartmentId",
                schema: "identity",
                table: "DepartmentClaims",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentReportingToRoleId",
                schema: "identity",
                table: "Departments",
                column: "DepartmentReportingToRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_CreatedAtUtc",
                schema: "identity",
                table: "OutboxEvents",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_WasSent",
                schema: "identity",
                table: "OutboxEvents",
                column: "WasSent");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignments_PermissionDefinitionId",
                schema: "identity",
                table: "PermissionAssignments",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAssignments_UserId_PermissionDefinitionId_ResourceId_ResourceType",
                schema: "identity",
                table: "PermissionAssignments",
                columns: ["UserId", "PermissionDefinitionId", "ResourceId", "ResourceType"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortalSettings_Name",
                schema: "identity",
                table: "PortalSettings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RedirectRules_RedirectKey",
                schema: "identity",
                table: "RedirectRules",
                column: "RedirectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceGroups_DepartmentId",
                schema: "identity",
                table: "ResourceGroups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_ClaimType",
                schema: "identity",
                table: "RoleClaims",
                column: "ClaimType");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_ClaimType",
                schema: "identity",
                table: "UserClaims",
                column: "ClaimType");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentHistory_DepartmentId",
                schema: "identity",
                table: "UserDepartmentHistory",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentHistory_UserId",
                schema: "identity",
                table: "UserDepartmentHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserId",
                schema: "identity",
                table: "UserMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                schema: "identity",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                schema: "identity",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResourceGroupId",
                schema: "identity",
                table: "Users",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ColorPicker",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "CustomDetails",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "DepartmentClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "MicrofrontendsConfigs",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "OutboxEvents",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "PermissionAssignments",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "PortalSettings",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "RedirectRules",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserDepartmentHistory",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserMessages",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "PermissionDefinitions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Claims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ClaimGroups",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ResourceGroups",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "identity");
        }
    }
}
