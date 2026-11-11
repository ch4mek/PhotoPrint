using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Inquiries_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inquiries");

            migrationBuilder.CreateTable(
                name: "CalendarExclusions",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    NewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DateType = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarExclusions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InquiriesUsers",
                schema: "inquiries",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FirstNameRu = table.Column<string>(type: "text", nullable: true),
                    LastNameRu = table.Column<string>(type: "text", nullable: true),
                    PatronymicRu = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "character varying(550)", maxLength: 550, nullable: false),
                    DepartmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RegisteredLocationId = table.Column<int>(type: "integer", nullable: true),
                    AvatarFileId = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    PositionOrderWeight = table.Column<int>(type: "integer", nullable: true),
                    ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
                    PeopleForceEmployeeId = table.Column<int>(type: "integer", nullable: true),
                    SynchronizedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiriesUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "InquiryTemplates",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    InquiryType = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    TrackingUnitType = table.Column<string>(type: "text", nullable: false),
                    ActivityType = table.Column<string>(type: "text", nullable: false),
                    InquiryTypeOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"InquiryType\" WHEN 'Vacation' THEN 100 WHEN 'SickDay' THEN 200 WHEN 'SickLeave' THEN 300 WHEN 'OwnExpense' THEN 400 WHEN 'HomeOffice' THEN 500 WHEN 'WeekendTransfer' THEN 600 WHEN 'MilitaryEnlistmentOffice' THEN 700 WHEN 'SocialVacation' THEN 800 WHEN 'Documents' THEN 900 WHEN 'OtherType' THEN 1000 WHEN 'Unknown' THEN 1100 ELSE 1100 END", stored: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ProhibitOnProbationPeriod = table.Column<bool>(type: "boolean", nullable: false),
                    RequireAttachments = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeHolidays = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeWeekends = table.Column<bool>(type: "boolean", nullable: false),
                    AccrualType = table.Column<string>(type: "text", nullable: true),
                    IsStaked = table.Column<bool>(type: "boolean", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    PeopleForceLeaveTypeId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryTemplates_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryTemplates_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InquiryTemplateAcceptPolicies",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InquiryTemplateId = table.Column<int>(type: "integer", nullable: false),
                    OrderWeight = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    SubstituteScope = table.Column<string>(type: "text", nullable: true),
                    SubstituteUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryTemplateAcceptPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryTemplateAcceptPolicies_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryTemplateAcceptPolicies_InquiriesUsers_SubstituteUser~",
                        column: x => x.SubstituteUserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryTemplateAcceptPolicies_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryTemplateAcceptPolicies_InquiriesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InquiryTemplateAcceptPolicies_InquiryTemplates_InquiryTempl~",
                        column: x => x.InquiryTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InquiryTemplateAgreements",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DaysPerContractYear = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    InquiryTemplateId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryTemplateAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryTemplateAgreements_InquiryTemplates_InquiryTemplateId",
                        column: x => x.InquiryTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiryTemplates",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: false),
                    InquiryTemplateId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiryTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiryTemplates_InquiriesUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryTemplates_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryTemplates_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryTemplates_InquiryTemplates_InquiryTemplateId",
                        column: x => x.InquiryTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAgreements",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    InquiryAgreementTemplateId = table.Column<int>(type: "integer", nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAgreements_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAgreements_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAgreements_InquiriesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAgreements_InquiryTemplateAgreements_InquiryAgreementTe~",
                        column: x => x.InquiryAgreementTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplateAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiries",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AgreementTemplateId = table.Column<int>(type: "integer", nullable: true),
                    InquiryTemplateId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StatusOrderWeight = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "CASE \"Status\" WHEN 'Approved' THEN 0 WHEN 'Rejected' THEN 100 WHEN 'Open' THEN 200 ELSE 200 END", stored: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalHours = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalDays = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalAccountantDays = table.Column<decimal>(type: "numeric", nullable: true),
                    CompensationAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    PeopleForceId = table.Column<int>(type: "integer", nullable: true),
                    HistoricalRemainingDays = table.Column<decimal>(type: "numeric", nullable: true),
                    HistoricalBalanceDays = table.Column<decimal>(type: "numeric", nullable: true),
                    HistoricalBalanceHours = table.Column<decimal>(type: "numeric", nullable: true),
                    HistoricalManagerId = table.Column<int>(type: "integer", nullable: true),
                    HistoricalManagerPosition = table.Column<string>(type: "text", nullable: true),
                    HistoricalUserPosition = table.Column<string>(type: "text", nullable: true),
                    HistoricalCreatedByUserPosition = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiries_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiries_InquiriesUsers_HistoricalManagerId",
                        column: x => x.HistoricalManagerId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UserInquiries_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiries_InquiriesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiries_InquiryTemplateAgreements_AgreementTemplateId",
                        column: x => x.AgreementTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplateAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiries_InquiryTemplates_InquiryTemplateId",
                        column: x => x.InquiryTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAgreementTransactions",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Desctiption = table.Column<string>(type: "text", nullable: true),
                    AmountOfDays = table.Column<decimal>(type: "numeric", nullable: false),
                    UserAgreementId = table.Column<int>(type: "integer", nullable: true),
                    InquiryTemplateId = table.Column<int>(type: "integer", nullable: false),
                    UserInquiryId = table.Column<int>(type: "integer", nullable: true),
                    IsCreatedManually = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAgreementTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAgreementTransactions_InquiriesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAgreementTransactions_InquiryTemplates_InquiryTemplateId",
                        column: x => x.InquiryTemplateId,
                        principalSchema: "inquiries",
                        principalTable: "InquiryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAgreementTransactions_UserAgreements_UserAgreementId",
                        column: x => x.UserAgreementId,
                        principalSchema: "inquiries",
                        principalTable: "UserAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAgreementTransactions_UserInquiries_UserInquiryId",
                        column: x => x.UserInquiryId,
                        principalSchema: "inquiries",
                        principalTable: "UserInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiryAccepts",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserInquiryId = table.Column<int>(type: "integer", nullable: true),
                    AcceptPolicyScope = table.Column<string>(type: "text", nullable: false),
                    AcceptUserId = table.Column<int>(type: "integer", nullable: false),
                    AcceptUserPosition = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AcceptedById = table.Column<int>(type: "integer", nullable: true),
                    AcceptedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectedById = table.Column<int>(type: "integer", nullable: true),
                    RejectedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiryAccepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiryAccepts_InquiriesUsers_AcceptUserId",
                        column: x => x.AcceptUserId,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryAccepts_InquiriesUsers_AcceptedById",
                        column: x => x.AcceptedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryAccepts_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryAccepts_InquiriesUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryAccepts_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryAccepts_UserInquiries_UserInquiryId",
                        column: x => x.UserInquiryId,
                        principalSchema: "inquiries",
                        principalTable: "UserInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiryComments",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: false),
                    UserInquiryId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiryComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiryComments_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryComments_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryComments_UserInquiries_UserInquiryId",
                        column: x => x.UserInquiryId,
                        principalSchema: "inquiries",
                        principalTable: "UserInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiryDates",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserInquiryCalendarDetailsId = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedDays = table.Column<decimal>(type: "numeric", nullable: false),
                    DateType = table.Column<string>(type: "text", nullable: false),
                    CalendarExclusionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiryDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiryDates_CalendarExclusions_CalendarExclusionId",
                        column: x => x.CalendarExclusionId,
                        principalSchema: "inquiries",
                        principalTable: "CalendarExclusions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryDates_UserInquiries_UserInquiryCalendarDetailsId",
                        column: x => x.UserInquiryCalendarDetailsId,
                        principalSchema: "inquiries",
                        principalTable: "UserInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiryFiles",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UserInquiryId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiryFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiryFiles_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryFiles_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryFiles_UserInquiries_UserInquiryId",
                        column: x => x.UserInquiryId,
                        principalSchema: "inquiries",
                        principalTable: "UserInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInquiryCommentFiles",
                schema: "inquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UserInquiryCommentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInquiryCommentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInquiryCommentFiles_InquiriesUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryCommentFiles_InquiriesUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalSchema: "inquiries",
                        principalTable: "InquiriesUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInquiryCommentFiles_UserInquiryComments_UserInquiryComm~",
                        column: x => x.UserInquiryCommentId,
                        principalSchema: "inquiries",
                        principalTable: "UserInquiryComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarExclusions_CountryId_Date",
                schema: "inquiries",
                table: "CalendarExclusions",
                columns: ["CountryId", "Date"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InquiriesUsers_PeopleForceEmployeeId",
                schema: "inquiries",
                table: "InquiriesUsers",
                column: "PeopleForceEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplateAcceptPolicies_CreatedById",
                schema: "inquiries",
                table: "InquiryTemplateAcceptPolicies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplateAcceptPolicies_InquiryTemplateId",
                schema: "inquiries",
                table: "InquiryTemplateAcceptPolicies",
                column: "InquiryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplateAcceptPolicies_SubstituteUserId",
                schema: "inquiries",
                table: "InquiryTemplateAcceptPolicies",
                column: "SubstituteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplateAcceptPolicies_UpdatedById",
                schema: "inquiries",
                table: "InquiryTemplateAcceptPolicies",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplateAcceptPolicies_UserId",
                schema: "inquiries",
                table: "InquiryTemplateAcceptPolicies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplateAgreements_InquiryTemplateId",
                schema: "inquiries",
                table: "InquiryTemplateAgreements",
                column: "InquiryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplates_CreatedById",
                schema: "inquiries",
                table: "InquiryTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryTemplates_UpdatedById",
                schema: "inquiries",
                table: "InquiryTemplates",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreements_CreatedById",
                schema: "inquiries",
                table: "UserAgreements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreements_InquiryAgreementTemplateId",
                schema: "inquiries",
                table: "UserAgreements",
                column: "InquiryAgreementTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreements_UpdatedById",
                schema: "inquiries",
                table: "UserAgreements",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreements_UserId_DateFrom_DateTo",
                schema: "inquiries",
                table: "UserAgreements",
                columns: ["UserId", "DateFrom", "DateTo"]);

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreementTransactions_InquiryTemplateId",
                schema: "inquiries",
                table: "UserAgreementTransactions",
                column: "InquiryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreementTransactions_UserAgreementId",
                schema: "inquiries",
                table: "UserAgreementTransactions",
                column: "UserAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreementTransactions_UserId_TransactionType",
                schema: "inquiries",
                table: "UserAgreementTransactions",
                columns: ["UserId", "TransactionType"]);

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreementTransactions_UserInquiryId",
                schema: "inquiries",
                table: "UserAgreementTransactions",
                column: "UserInquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_AgreementTemplateId",
                schema: "inquiries",
                table: "UserInquiries",
                column: "AgreementTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_CreatedById",
                schema: "inquiries",
                table: "UserInquiries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_HistoricalManagerId",
                schema: "inquiries",
                table: "UserInquiries",
                column: "HistoricalManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_InquiryTemplateId",
                schema: "inquiries",
                table: "UserInquiries",
                column: "InquiryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_PeopleForceId",
                schema: "inquiries",
                table: "UserInquiries",
                column: "PeopleForceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_UpdatedById",
                schema: "inquiries",
                table: "UserInquiries",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiries_UserId",
                schema: "inquiries",
                table: "UserInquiries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryAccepts_AcceptedById",
                schema: "inquiries",
                table: "UserInquiryAccepts",
                column: "AcceptedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryAccepts_AcceptUserId",
                schema: "inquiries",
                table: "UserInquiryAccepts",
                column: "AcceptUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryAccepts_CreatedById",
                schema: "inquiries",
                table: "UserInquiryAccepts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryAccepts_RejectedById",
                schema: "inquiries",
                table: "UserInquiryAccepts",
                column: "RejectedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryAccepts_UpdatedById",
                schema: "inquiries",
                table: "UserInquiryAccepts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryAccepts_UserInquiryId",
                schema: "inquiries",
                table: "UserInquiryAccepts",
                column: "UserInquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryCommentFiles_CreatedById",
                schema: "inquiries",
                table: "UserInquiryCommentFiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryCommentFiles_UpdatedById",
                schema: "inquiries",
                table: "UserInquiryCommentFiles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryCommentFiles_UserInquiryCommentId",
                schema: "inquiries",
                table: "UserInquiryCommentFiles",
                column: "UserInquiryCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryComments_CreatedById",
                schema: "inquiries",
                table: "UserInquiryComments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryComments_UpdatedById",
                schema: "inquiries",
                table: "UserInquiryComments",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryComments_UserInquiryId",
                schema: "inquiries",
                table: "UserInquiryComments",
                column: "UserInquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryDates_CalendarExclusionId",
                schema: "inquiries",
                table: "UserInquiryDates",
                column: "CalendarExclusionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryDates_UserInquiryCalendarDetailsId",
                schema: "inquiries",
                table: "UserInquiryDates",
                column: "UserInquiryCalendarDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryFiles_CreatedById",
                schema: "inquiries",
                table: "UserInquiryFiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryFiles_UpdatedById",
                schema: "inquiries",
                table: "UserInquiryFiles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryFiles_UserInquiryId",
                schema: "inquiries",
                table: "UserInquiryFiles",
                column: "UserInquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryTemplates_AssignedToUserId",
                schema: "inquiries",
                table: "UserInquiryTemplates",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryTemplates_CreatedById",
                schema: "inquiries",
                table: "UserInquiryTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryTemplates_InquiryTemplateId",
                schema: "inquiries",
                table: "UserInquiryTemplates",
                column: "InquiryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInquiryTemplates_UpdatedById",
                schema: "inquiries",
                table: "UserInquiryTemplates",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquiryTemplateAcceptPolicies",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserAgreementTransactions",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiryAccepts",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiryCommentFiles",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiryDates",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiryFiles",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiryTemplates",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserAgreements",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiryComments",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "CalendarExclusions",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "UserInquiries",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "InquiryTemplateAgreements",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "InquiryTemplates",
                schema: "inquiries");

            migrationBuilder.DropTable(
                name: "InquiriesUsers",
                schema: "inquiries");
        }
    }
}
