using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "teamboard");

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnglishLevels",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    InternalName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnglishLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HardSkillGroups",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardSkillGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KpiColors",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RightIntervalBorder = table.Column<int>(type: "integer", nullable: false),
                    ColorOrderWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiColors", x => x.Id);
                    table.CheckConstraint("CK_KpiColors_RightIntervalBorder", "\"RightIntervalBorder\" <= 9999 AND \"RightIntervalBorder\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                schema: "teamboard",
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
                name: "ProjectExitClosingReasons",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExitClosingReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeniorityLevels",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    InternalName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    Coefficient = table.Column<int>(type: "integer", nullable: false),
                    SpecializationRangeTo = table.Column<double>(type: "double precision", nullable: false),
                    SeniorityLevelScope = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeniorityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamBoardClients",
                schema: "teamboard",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamBoardClients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "TeamboardDepartments",
                schema: "teamboard",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DepartmentOrderWeight = table.Column<int>(type: "integer", nullable: false),
                    DepartmentReportingToRoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamboardDepartments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "TeamboardResourceGroups",
                schema: "teamboard",
                columns: table => new
                {
                    ResourceGroupId = table.Column<int>(type: "integer", maxLength: 64, nullable: false),
                    ResourceGroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamboardResourceGroups", x => x.ResourceGroupId);
                });

            migrationBuilder.CreateTable(
                name: "WorkingHoursPerMonths",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    MonthOfYear = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    Hours = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingHoursPerMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkingHoursPerMonths_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "teamboard",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamBoardUsers",
                schema: "teamboard",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastNameRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PatronymicRu = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PositionOrderWeight = table.Column<int>(type: "integer", nullable: true),
                    CurrentLocation = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AvatarColor = table.Column<string>(type: "text", nullable: true),
                    AvatarFileId = table.Column<string>(type: "text", nullable: true),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    EnglishLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    Comments = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CareerStartedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    FirstProjectInCompanyStartedAtUtc = table.Column<DateOnly>(type: "date", nullable: true),
                    PersonalEmail = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    PersonalEmailSecond = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    PersonalPhone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Telegram = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LinkedIn = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: true),
                    MilitaryServiceStatus = table.Column<string>(type: "text", nullable: true),
                    IsGraduateJobPlacement = table.Column<bool>(type: "boolean", nullable: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    SynchronizedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamBoardUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_TeamBoardUsers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "teamboard",
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamBoardUsers_EnglishLevels_EnglishLevelId",
                        column: x => x.EnglishLevelId,
                        principalSchema: "teamboard",
                        principalTable: "EnglishLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamBoardUsers_SeniorityLevels_SeniorityLevelId",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamBoardUsers_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamBoardUsers_TeamboardDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "teamboard",
                        principalTable: "TeamboardDepartments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamBoardUsers_TeamboardResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalSchema: "teamboard",
                        principalTable: "TeamboardResourceGroups",
                        principalColumn: "ResourceGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    ConversationType = table.Column<string>(type: "text", nullable: false),
                    ConversationResult = table.Column<int>(type: "integer", nullable: true),
                    Theses = table.Column<string>(type: "text", nullable: true),
                    EmployeeTheses = table.Column<string>(type: "text", nullable: true),
                    Agreements = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    SupervisorId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_TeamBoardUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimatedUserMonthPayloads",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PayloadYear = table.Column<int>(type: "integer", nullable: false),
                    PayloadMonthOfYear = table.Column<int>(type: "integer", nullable: false),
                    EstimatedFte = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimatedUserMonthPayloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimatedUserMonthPayloads_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstimatedUserMonthPayloads_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstimatedUserMonthPayloads_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HardSkills",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    HardSkillGroupId = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HardSkills_HardSkillGroups_HardSkillGroupId",
                        column: x => x.HardSkillGroupId,
                        principalSchema: "teamboard",
                        principalTable: "HardSkillGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HardSkills_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payslips",
                schema: "teamboard",
                columns: table => new
                {
                    FileId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FileName = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    ReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_Payslips_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredLocationHistory",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RegisteredLocationId = table.Column<int>(type: "integer", nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredLocationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisteredLocationHistory_Countries_RegisteredLocationId",
                        column: x => x.RegisteredLocationId,
                        principalSchema: "teamboard",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegisteredLocationHistory_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoftSkills",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftSkills_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationTemplates",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecializationTemplates_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecializationTemplates_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamBoardProjects",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ClientName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Color = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamBoardProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamBoardProjects_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserContracts",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ContractFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ContractTo = table.Column<DateOnly>(type: "date", nullable: true),
                    FireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FireType = table.Column<string>(type: "text", nullable: true),
                    FireReason = table.Column<string>(type: "text", nullable: true),
                    FireDescription = table.Column<string>(type: "text", nullable: true),
                    IsProbationPeriod = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContracts_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserContracts_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserContracts_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserEducations",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EducationLevel = table.Column<string>(type: "text", nullable: false),
                    EducationalInstitution = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Faculty = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Specialization = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    StudyForm = table.Column<string>(type: "text", nullable: false),
                    GraduationYear = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EducationLevelOrderWeight = table.Column<int>(type: "integer", nullable: false),
                    StudyFormOrderWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEducations", x => x.Id);
                    table.CheckConstraint("CK_Educations_GraduationYear", "\"GraduationYear\" <= 9999 AND \"GraduationYear\" > 0");
                    table.ForeignKey(
                        name: "FK_UserEducations_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserEducations_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserEducations_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInterviews",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Specialization = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UserInterviewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UserInterviewFromUtc = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    UserInterviewToUtc = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ClientName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    StatusOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"Status\" WHEN 'WaitingForInterview' THEN 100 WHEN 'WaitingForResult' THEN 200 WHEN 'Failed' THEN 300 WHEN 'Success' THEN 400 WHEN 'Started' THEN 500 WHEN 'NotStarted' THEN 600 ELSE 700 END", stored: true),
                    InterviewType = table.Column<string>(type: "text", nullable: true),
                    InterviewTypeOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"InterviewType\" WHEN 'Prescreen' THEN 100 WHEN 'Tech' THEN 200 WHEN 'Final' THEN 300 WHEN 'Other' THEN 400 ELSE 500 END", stored: true),
                    SalesResponsibleUserId = table.Column<int>(type: "integer", nullable: true),
                    JiraTask = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReasonForRefusal = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true),
                    RequiredLocation = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    RequiredEnglishLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInterviews_EnglishLevels_RequiredEnglishLevelId",
                        column: x => x.RequiredEnglishLevelId,
                        principalSchema: "teamboard",
                        principalTable: "EnglishLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInterviews_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInterviews_TeamBoardUsers_SalesResponsibleUserId",
                        column: x => x.SalesResponsibleUserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInterviews_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInterviews_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPayloadDetails",
                schema: "teamboard",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Plan = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NeedProject = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPayloadDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPayloadDetails_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPayloadDetails_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPayloadDetails_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSkillsSnapshots",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SnapshotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkillsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshots_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshots_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshots_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSoftSkillSnapshots",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SnapshotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSoftSkillSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSoftSkillSnapshots_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSoftSkillSnapshots_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSoftSkillSnapshots_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConversationDocumentLinks",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Link = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationDocumentLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationDocumentLinks_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "teamboard",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConversationFiles",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationFiles_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "teamboard",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DevelopmentPlans",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeadlineDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "None"),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ConversationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevelopmentPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevelopmentPlans_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "teamboard",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevelopmentPlans_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPerformanceReviews",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    InterviewDateFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InterviewDateToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EmployeeFeedback = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Agreements = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ResponsibleUserId = table.Column<int>(type: "integer", nullable: true),
                    ResponsibleUserFeedback = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ChiefLevelManagerId = table.Column<int>(type: "integer", nullable: true),
                    ChiefLevelManagerFeedback = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConversationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPerformanceReviews", x => x.Id);
                    table.CheckConstraint("CK_UserPerformanceReview_InterviewDateTimeRange", "\"InterviewDateToUtc\" > \"InterviewDateFromUtc\"");
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviews_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "teamboard",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviews_TeamBoardUsers_ChiefLevelManagerId",
                        column: x => x.ChiefLevelManagerId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviews_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviews_TeamBoardUsers_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviews_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviews_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSalaries",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SalaryFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    SalaryUsd = table.Column<string>(type: "text", nullable: false),
                    SalaryInCurrency = table.Column<string>(type: "text", nullable: false),
                    SalaryCurrencyId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    ConversationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSalaries_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "teamboard",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSalaries_Currencies_SalaryCurrencyId",
                        column: x => x.SalaryCurrencyId,
                        principalSchema: "teamboard",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSalaries_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSalaries_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationTemplateDepartments",
                schema: "teamboard",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "text", nullable: false),
                    SpecializationTemplateId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationTemplateDepartments", x => new { x.SpecializationTemplateId, x.DepartmentId });
                    table.ForeignKey(
                        name: "FK_SpecializationTemplateDepartments_SpecializationTemplates_S~",
                        column: x => x.SpecializationTemplateId,
                        principalSchema: "teamboard",
                        principalTable: "SpecializationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecializationTemplateDepartments_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationTemplateHardSkills",
                schema: "teamboard",
                columns: table => new
                {
                    HardSkillId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationTemplateId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationTemplateHardSkills", x => new { x.SpecializationTemplateId, x.HardSkillId });
                    table.ForeignKey(
                        name: "FK_SpecializationTemplateHardSkills_HardSkills_HardSkillId",
                        column: x => x.HardSkillId,
                        principalSchema: "teamboard",
                        principalTable: "HardSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecializationTemplateHardSkills_SpecializationTemplates_Sp~",
                        column: x => x.SpecializationTemplateId,
                        principalSchema: "teamboard",
                        principalTable: "SpecializationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecializationTemplateHardSkills_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSpecializations",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationTemplateId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleSpecialization = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleSeniorityLevel = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSpecializations_SeniorityLevels_SeniorityLevelId",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializations_SpecializationTemplates_SpecializationT~",
                        column: x => x.SpecializationTemplateId,
                        principalSchema: "teamboard",
                        principalTable: "SpecializationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializations_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializations_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalFeedbacks",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalFeedbackDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: false),
                    ActionsTodo = table.Column<string>(type: "text", nullable: true),
                    ExternalFeedbackType = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ConversationId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Reviewer = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalFeedbacks_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "teamboard",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalFeedbacks_TeamBoardProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalFeedbacks_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalFeedbacks_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectExits",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ClosingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ClosingReasonId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectExits_ProjectExitClosingReasons_ClosingReasonId",
                        column: x => x.ClosingReasonId,
                        principalSchema: "teamboard",
                        principalTable: "ProjectExitClosingReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExits_TeamBoardClients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardClients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExits_TeamBoardProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExits_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExits_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExits_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserOnProjectFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    UserOnProjectTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsTimeOnProjectBillable = table.Column<bool>(type: "boolean", nullable: false),
                    UserOnProjectSeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    Fte = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    RefUserId = table.Column<int>(type: "integer", nullable: true),
                    Comments = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_SeniorityLevels_UserOnProjectSeniorityLevelId",
                        column: x => x.UserOnProjectSeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TeamBoardProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TeamBoardUsers_RefUserId",
                        column: x => x.RefUserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInterviewFeedbacks",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserInterviewId = table.Column<int>(type: "integer", nullable: false),
                    FeedbackDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ActionsTodo = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterviewFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInterviewFeedbacks_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInterviewFeedbacks_UserInterviews_UserInterviewId",
                        column: x => x.UserInterviewId,
                        principalSchema: "teamboard",
                        principalTable: "UserInterviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSkillsSnapshotSpecializations",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserSkillsSnapshotId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationTemplateId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserSpecializationWasPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkillsSnapshotSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshotSpecializations_SeniorityLevels_Seniority~",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshotSpecializations_SpecializationTemplates_S~",
                        column: x => x.SpecializationTemplateId,
                        principalSchema: "teamboard",
                        principalTable: "SpecializationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshotSpecializations_UserSkillsSnapshots_UserS~",
                        column: x => x.UserSkillsSnapshotId,
                        principalSchema: "teamboard",
                        principalTable: "UserSkillsSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSoftSkills",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SoftSkillId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    IsVisibleSoftSkill = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleSeniorityLevel = table.Column<bool>(type: "boolean", nullable: false),
                    UserSoftSkillsSnapshotId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSoftSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSoftSkills_SeniorityLevels_SeniorityLevelId",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSoftSkills_SoftSkills_SoftSkillId",
                        column: x => x.SoftSkillId,
                        principalSchema: "teamboard",
                        principalTable: "SoftSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSoftSkills_TeamBoardUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSoftSkills_UserSoftSkillSnapshots_UserSoftSkillsSnapsho~",
                        column: x => x.UserSoftSkillsSnapshotId,
                        principalSchema: "teamboard",
                        principalTable: "UserSoftSkillSnapshots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPerformanceReviewGoals",
                schema: "teamboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserPerformanceReviewId = table.Column<int>(type: "integer", nullable: false),
                    GoalDescription = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Deadline = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerifiedById = table.Column<int>(type: "integer", nullable: true),
                    VerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPerformanceReviewGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviewGoals_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviewGoals_TeamBoardUsers_VerifiedById",
                        column: x => x.VerifiedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPerformanceReviewGoals_UserPerformanceReviews_UserPerfo~",
                        column: x => x.UserPerformanceReviewId,
                        principalSchema: "teamboard",
                        principalTable: "UserPerformanceReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSpecializationHardSkills",
                schema: "teamboard",
                columns: table => new
                {
                    UserSpecializationId = table.Column<int>(type: "integer", nullable: false),
                    HardSkillId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", nullable: true),
                    IsVisibleHardSkill = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleSeniorityLevel = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpecializationHardSkills", x => new { x.UserSpecializationId, x.HardSkillId });
                    table.ForeignKey(
                        name: "FK_UserSpecializationHardSkills_HardSkills_HardSkillId",
                        column: x => x.HardSkillId,
                        principalSchema: "teamboard",
                        principalTable: "HardSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializationHardSkills_SeniorityLevels_SeniorityLevel~",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializationHardSkills_TeamBoardUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializationHardSkills_TeamBoardUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "teamboard",
                        principalTable: "TeamBoardUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSpecializationHardSkills_UserSpecializations_UserSpecia~",
                        column: x => x.UserSpecializationId,
                        principalSchema: "teamboard",
                        principalTable: "UserSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSkillsSnapshotSpecializationHardSkills",
                schema: "teamboard",
                columns: table => new
                {
                    UserSkillsSnapshotSpecializationId = table.Column<int>(type: "integer", nullable: false),
                    HardSkillId = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevelId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkillsSnapshotSpecializationHardSkills", x => new { x.UserSkillsSnapshotSpecializationId, x.HardSkillId });
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshotSpecializationHardSkills_HardSkills_HardS~",
                        column: x => x.HardSkillId,
                        principalSchema: "teamboard",
                        principalTable: "HardSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshotSpecializationHardSkills_SeniorityLevels_~",
                        column: x => x.SeniorityLevelId,
                        principalSchema: "teamboard",
                        principalTable: "SeniorityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSkillsSnapshotSpecializationHardSkills_UserSkillsSnapsh~",
                        column: x => x.UserSkillsSnapshotSpecializationId,
                        principalSchema: "teamboard",
                        principalTable: "UserSkillsSnapshotSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                     CREATE OR REPLACE FUNCTION teamboard.round_seniority(value DOUBLE PRECISION)
                     RETURNS DOUBLE PRECISION AS $$
                     DECLARE
                         tenths INTEGER;
                         integer_part INTEGER;
                     BEGIN
                         tenths := CAST(FLOOR(value * 10) AS INTEGER) % 10;
                         integer_part := CAST(FLOOR(value) AS INTEGER);

                         IF tenths >= 6 AND tenths <= 9 THEN
                             RETURN CEIL(value);
                         ELSE
                             RETURN FLOOR(value);
                         END IF;
                     END;
                     $$ LANGUAGE plpgsql;

                     CREATE OR REPLACE FUNCTION teamboard.get_average_soft_skill_seniority(user_id INT)
                     RETURNS DOUBLE PRECISION AS $$
                     DECLARE
                         numinator DOUBLE PRECISION;
                         denominator INTEGER;
                         obtained_value DOUBLE PRECISION;
                         rounded_value DOUBLE PRECISION;
                     BEGIN
                         SELECT SUM(sl."Coefficient"), COUNT(*)
                         INTO numinator, denominator
                         FROM "teamboard"."UserSoftSkills" uss
                         JOIN "teamboard"."SeniorityLevels" sl ON uss."SeniorityLevelId" = sl."Id"
                         WHERE uss."UserId" = user_id AND uss."SeniorityLevelId" IS NOT NULL;

                         IF denominator = 0 THEN
                             RETURN NULL;
                         END IF;

                         obtained_value := numinator / denominator;
                         rounded_value := teamboard.round_seniority(obtained_value);

                         RETURN rounded_value;
                     END;
                     $$ LANGUAGE plpgsql;

                     CREATE OR REPLACE FUNCTION teamboard.get_user_general_seniority(user_id INT)
                     RETURNS CHAR VARYING
                     AS $$
                     DECLARE
                        average_soft_skill_seniority_value DOUBLE PRECISION;
                        general_seniority_value DOUBLE PRECISION;
                        general_seniority DOUBLE PRECISION;
                        seniority_level CHAR VARYING;
                        primary_specialization_seniority_coefficient INT;
                     BEGIN
                        average_soft_skill_seniority_value := teamboard.get_average_soft_skill_seniority(user_id);

                        IF average_soft_skill_seniority_value IS NOT NULL THEN

                            SELECT sl."Coefficient" INTO primary_specialization_seniority_coefficient
                            FROM "teamboard"."UserSpecializations" us
                            LEFT JOIN "teamboard"."SeniorityLevels" sl ON us."SeniorityLevelId" = sl."Id"  
                            WHERE us."UserId"=user_id AND us."IsPrimary" = true;

                            general_seniority_value := (average_soft_skill_seniority_value + primary_specialization_seniority_coefficient) / 2;

                            general_seniority := teamboard.round_seniority(general_seniority_value);

                            -- Get the seniority level by coefficient
                            SELECT "ExternalName" INTO seniority_level
                            FROM "teamboard"."SeniorityLevels"
                            WHERE "Coefficient" = general_seniority
                            LIMIT 1;

                            RETURN seniority_level;
                        ELSE
                            RETURN NULL;
                        END IF;
                     END;
                     $$
                     LANGUAGE plpgsql;
                     """);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationDocumentLinks_ConversationId",
                schema: "teamboard",
                table: "ConversationDocumentLinks",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationFiles_ConversationId",
                schema: "teamboard",
                table: "ConversationFiles",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_SupervisorId",
                schema: "teamboard",
                table: "Conversations",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserId",
                schema: "teamboard",
                table: "Conversations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DevelopmentPlans_ConversationId",
                schema: "teamboard",
                table: "DevelopmentPlans",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_DevelopmentPlans_UserId",
                schema: "teamboard",
                table: "DevelopmentPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EnglishLevels_OrderWeight",
                schema: "teamboard",
                table: "EnglishLevels",
                column: "OrderWeight");

            migrationBuilder.CreateIndex(
                name: "IX_EstimatedUserMonthPayloads_CreatedById",
                schema: "teamboard",
                table: "EstimatedUserMonthPayloads",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EstimatedUserMonthPayloads_UpdatedById",
                schema: "teamboard",
                table: "EstimatedUserMonthPayloads",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EstimatedUserMonthPayloads_UserId_PayloadYear",
                schema: "teamboard",
                table: "EstimatedUserMonthPayloads",
                columns: ["UserId", "PayloadYear"]);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalFeedbacks_ConversationId",
                schema: "teamboard",
                table: "ExternalFeedbacks",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalFeedbacks_CreatedById",
                schema: "teamboard",
                table: "ExternalFeedbacks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalFeedbacks_ProjectId",
                schema: "teamboard",
                table: "ExternalFeedbacks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalFeedbacks_UserId",
                schema: "teamboard",
                table: "ExternalFeedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HardSkillGroups_Id",
                schema: "teamboard",
                table: "HardSkillGroups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HardSkills_CreatedById",
                schema: "teamboard",
                table: "HardSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HardSkills_HardSkillGroupId",
                schema: "teamboard",
                table: "HardSkills",
                column: "HardSkillGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_CreatedAtUtc",
                schema: "teamboard",
                table: "OutboxEvents",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_WasSent",
                schema: "teamboard",
                table: "OutboxEvents",
                column: "WasSent");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_UserId",
                schema: "teamboard",
                table: "Payslips",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExitClosingReasons_Name",
                schema: "teamboard",
                table: "ProjectExitClosingReasons",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExits_ClientId",
                schema: "teamboard",
                table: "ProjectExits",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExits_ClosingReasonId",
                schema: "teamboard",
                table: "ProjectExits",
                column: "ClosingReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExits_CreatedById",
                schema: "teamboard",
                table: "ProjectExits",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExits_ProjectId",
                schema: "teamboard",
                table: "ProjectExits",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExits_UpdatedById",
                schema: "teamboard",
                table: "ProjectExits",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExits_UserId",
                schema: "teamboard",
                table: "ProjectExits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectId",
                schema: "teamboard",
                table: "ProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_RefUserId",
                schema: "teamboard",
                table: "ProjectUsers",
                column: "RefUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UpdatedById",
                schema: "teamboard",
                table: "ProjectUsers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                schema: "teamboard",
                table: "ProjectUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserOnProjectFrom_UserOnProjectTo",
                schema: "teamboard",
                table: "ProjectUsers",
                columns: ["UserOnProjectFrom", "UserOnProjectTo"])
                .Annotation("Npgsql:IndexInclude", new[] { "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserOnProjectSeniorityLevelId",
                schema: "teamboard",
                table: "ProjectUsers",
                column: "UserOnProjectSeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredLocationHistory_RegisteredLocationId",
                schema: "teamboard",
                table: "RegisteredLocationHistory",
                column: "RegisteredLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredLocationHistory_UserId",
                schema: "teamboard",
                table: "RegisteredLocationHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SeniorityLevels_Coefficient",
                schema: "teamboard",
                table: "SeniorityLevels",
                column: "Coefficient");

            migrationBuilder.CreateIndex(
                name: "IX_SeniorityLevels_OrderWeight",
                schema: "teamboard",
                table: "SeniorityLevels",
                column: "OrderWeight");

            migrationBuilder.CreateIndex(
                name: "IX_SoftSkills_CreatedById",
                schema: "teamboard",
                table: "SoftSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationTemplateDepartments_CreatedById",
                schema: "teamboard",
                table: "SpecializationTemplateDepartments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationTemplateHardSkills_CreatedById",
                schema: "teamboard",
                table: "SpecializationTemplateHardSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationTemplateHardSkills_HardSkillId",
                schema: "teamboard",
                table: "SpecializationTemplateHardSkills",
                column: "HardSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationTemplates_CreatedById",
                schema: "teamboard",
                table: "SpecializationTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationTemplates_UpdatedById",
                schema: "teamboard",
                table: "SpecializationTemplates",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardClients_Name",
                schema: "teamboard",
                table: "TeamBoardClients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardProjects_UpdatedById",
                schema: "teamboard",
                table: "TeamBoardProjects",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_CountryId",
                schema: "teamboard",
                table: "TeamBoardUsers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_DepartmentId",
                schema: "teamboard",
                table: "TeamBoardUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_EnglishLevelId",
                schema: "teamboard",
                table: "TeamBoardUsers",
                column: "EnglishLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_IsActive_DepartmentId",
                schema: "teamboard",
                table: "TeamBoardUsers",
                columns: ["IsActive", "DepartmentId"]);

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_ResourceGroupId",
                schema: "teamboard",
                table: "TeamBoardUsers",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_SeniorityLevelId",
                schema: "teamboard",
                table: "TeamBoardUsers",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoardUsers_UpdatedById",
                schema: "teamboard",
                table: "TeamBoardUsers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserContracts_CreatedById",
                schema: "teamboard",
                table: "UserContracts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserContracts_UpdatedById",
                schema: "teamboard",
                table: "UserContracts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserContracts_UserId",
                schema: "teamboard",
                table: "UserContracts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEducations_CreatedById",
                schema: "teamboard",
                table: "UserEducations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserEducations_UpdatedById",
                schema: "teamboard",
                table: "UserEducations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserEducations_UserId",
                schema: "teamboard",
                table: "UserEducations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviewFeedbacks_CreatedById",
                schema: "teamboard",
                table: "UserInterviewFeedbacks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviewFeedbacks_UserInterviewId",
                schema: "teamboard",
                table: "UserInterviewFeedbacks",
                column: "UserInterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviews_CreatedById",
                schema: "teamboard",
                table: "UserInterviews",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviews_GroupId",
                schema: "teamboard",
                table: "UserInterviews",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviews_RequiredEnglishLevelId",
                schema: "teamboard",
                table: "UserInterviews",
                column: "RequiredEnglishLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviews_SalesResponsibleUserId",
                schema: "teamboard",
                table: "UserInterviews",
                column: "SalesResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviews_UpdatedById",
                schema: "teamboard",
                table: "UserInterviews",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterviews_UserId",
                schema: "teamboard",
                table: "UserInterviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPayloadDetails_CreatedById",
                schema: "teamboard",
                table: "UserPayloadDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPayloadDetails_UpdatedById",
                schema: "teamboard",
                table: "UserPayloadDetails",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviewGoals_CreatedById",
                schema: "teamboard",
                table: "UserPerformanceReviewGoals",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviewGoals_UserPerformanceReviewId",
                schema: "teamboard",
                table: "UserPerformanceReviewGoals",
                column: "UserPerformanceReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviewGoals_VerifiedById",
                schema: "teamboard",
                table: "UserPerformanceReviewGoals",
                column: "VerifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviews_ChiefLevelManagerId",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                column: "ChiefLevelManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviews_ConversationId",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviews_CreatedById",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviews_ResponsibleUserId",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviews_UpdatedById",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPerformanceReviews_UserId",
                schema: "teamboard",
                table: "UserPerformanceReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSalaries_ConversationId",
                schema: "teamboard",
                table: "UserSalaries",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSalaries_CreatedById",
                schema: "teamboard",
                table: "UserSalaries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSalaries_SalaryCurrencyId",
                schema: "teamboard",
                table: "UserSalaries",
                column: "SalaryCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSalaries_UserId",
                schema: "teamboard",
                table: "UserSalaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshots_CreatedById",
                schema: "teamboard",
                table: "UserSkillsSnapshots",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshots_UpdatedById",
                schema: "teamboard",
                table: "UserSkillsSnapshots",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshots_UserId",
                schema: "teamboard",
                table: "UserSkillsSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshotSpecializationHardSkills_HardSkillId",
                schema: "teamboard",
                table: "UserSkillsSnapshotSpecializationHardSkills",
                column: "HardSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshotSpecializationHardSkills_SeniorityLevelId",
                schema: "teamboard",
                table: "UserSkillsSnapshotSpecializationHardSkills",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshotSpecializations_SeniorityLevelId",
                schema: "teamboard",
                table: "UserSkillsSnapshotSpecializations",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshotSpecializations_SpecializationTemplateId",
                schema: "teamboard",
                table: "UserSkillsSnapshotSpecializations",
                column: "SpecializationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkillsSnapshotSpecializations_UserSkillsSnapshotId_Spec~",
                schema: "teamboard",
                table: "UserSkillsSnapshotSpecializations",
                columns: ["UserSkillsSnapshotId", "SpecializationTemplateId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkills_SeniorityLevelId",
                schema: "teamboard",
                table: "UserSoftSkills",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkills_SoftSkillId",
                schema: "teamboard",
                table: "UserSoftSkills",
                column: "SoftSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkills_UserId",
                schema: "teamboard",
                table: "UserSoftSkills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkills_UserSoftSkillsSnapshotId",
                schema: "teamboard",
                table: "UserSoftSkills",
                column: "UserSoftSkillsSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkillSnapshots_CreatedById",
                schema: "teamboard",
                table: "UserSoftSkillSnapshots",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkillSnapshots_UpdatedById",
                schema: "teamboard",
                table: "UserSoftSkillSnapshots",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSoftSkillSnapshots_UserId",
                schema: "teamboard",
                table: "UserSoftSkillSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializationHardSkills_CreatedById",
                schema: "teamboard",
                table: "UserSpecializationHardSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializationHardSkills_HardSkillId",
                schema: "teamboard",
                table: "UserSpecializationHardSkills",
                column: "HardSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializationHardSkills_SeniorityLevelId",
                schema: "teamboard",
                table: "UserSpecializationHardSkills",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializationHardSkills_UpdatedById",
                schema: "teamboard",
                table: "UserSpecializationHardSkills",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializations_CreatedById",
                schema: "teamboard",
                table: "UserSpecializations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializations_SeniorityLevelId",
                schema: "teamboard",
                table: "UserSpecializations",
                column: "SeniorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializations_SpecializationTemplateId",
                schema: "teamboard",
                table: "UserSpecializations",
                column: "SpecializationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializations_UserId_SpecializationTemplateId",
                schema: "teamboard",
                table: "UserSpecializations",
                columns: ["UserId", "SpecializationTemplateId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHoursPerMonths_CountryId",
                schema: "teamboard",
                table: "WorkingHoursPerMonths",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHoursPerMonths_Year_MonthOfYear_CountryId",
                schema: "teamboard",
                table: "WorkingHoursPerMonths",
                columns: ["Year", "MonthOfYear", "CountryId"],
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationDocumentLinks",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "ConversationFiles",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "DevelopmentPlans",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "EstimatedUserMonthPayloads",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "ExternalFeedbacks",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "KpiColors",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "OutboxEvents",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "Payslips",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "ProjectExits",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "ProjectUsers",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "RegisteredLocationHistory",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "SpecializationTemplateDepartments",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "SpecializationTemplateHardSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserContracts",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserEducations",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserInterviewFeedbacks",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserPayloadDetails",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserPerformanceReviewGoals",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSalaries",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSkillsSnapshotSpecializationHardSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSoftSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSpecializationHardSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "WorkingHoursPerMonths",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "ProjectExitClosingReasons",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "TeamBoardClients",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "TeamBoardProjects",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserInterviews",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserPerformanceReviews",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSkillsSnapshotSpecializations",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "SoftSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSoftSkillSnapshots",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "HardSkills",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSpecializations",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "Conversations",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "UserSkillsSnapshots",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "HardSkillGroups",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "SpecializationTemplates",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "TeamBoardUsers",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "EnglishLevels",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "SeniorityLevels",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "TeamboardDepartments",
                schema: "teamboard");

            migrationBuilder.DropTable(
                name: "TeamboardResourceGroups",
                schema: "teamboard");
        }
    }
}
