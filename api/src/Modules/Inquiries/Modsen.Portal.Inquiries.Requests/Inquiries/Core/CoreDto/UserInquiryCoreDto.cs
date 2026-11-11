using System.Linq.Expressions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

internal class UserInquiryCoreDto
{
    public int Id { get; set; }

    public int InquiryUserId { get; set; }

    public int? AgreementTemplateId { get; set; }
    public int InquiryTemplateId { get; set; }
    public required string InquiryTemplateName { get; set; }
    public required string InquiryType { get; set; }
    public int InquiryTypeOrderWeight { get; set; }
    public bool IsAgreement { get; set; }

    public UserInquiryStatus Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal? TotalHours { get; set; }
    public decimal? TotalDays { get; set; }
    public decimal? TotalAccountantDays { get; set; }
    public decimal? CompensationAmount { get; set; }

    public TrackingUnit TrackingUnitType { get; set; }
    public decimal? HistoricalRemainingDays { get; set; }
    public decimal? HistoricalBalanceDays { get; set; }
    public decimal? HistoricalBalanceHours { get; set; }
    public bool CanCurrentUserApprove { get; set; }
    public bool CanCurrentUserReject { get; set; }
    public int CountryId { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public required InquiriesUserDto CreatedByUser { get; set; }
    public required InquiriesUserDto CreatedForUser { get; set; }
    public InquiriesUserDto? ManagerOfUser { get; set; }

    public List<UserInquiryFileCoreDto> UserInquiryFiles { get; set; } = [];
    public List<UserInquiryDateCoreDto> UserInquiryDates { get; set; } = [];
    public List<UserInquiryAcceptCoreDto> UserInquiryAccepts { get; set; } = [];

    public static readonly Expression<Func<UserInquiryEntity, UserInquiryCoreDto>> Map =
        userInquiry => new UserInquiryCoreDto
        {
            Id = userInquiry.Id,
            InquiryUserId = userInquiry.UserId,
            AgreementTemplateId = userInquiry.AgreementTemplateId,
            InquiryTemplateId = userInquiry.InquiryTemplateId,
            InquiryTemplateName = userInquiry.InquiryTemplate!.Name,
            InquiryTypeOrderWeight = userInquiry.InquiryTemplate.InquiryTypeOrderWeight,
            IsAgreement = userInquiry.AgreementTemplateId != null,
            Status = userInquiry.Status,
            StatusOrderWeight = userInquiry.StatusOrderWeight,
            Notes = userInquiry.Notes,
            DateFrom = userInquiry.DateFrom,
            DateTo = userInquiry.DateTo,
            TotalDays = userInquiry.TotalDays,
            TotalHours = userInquiry.TotalHours,
            TotalAccountantDays = userInquiry.TotalAccountantDays,
            CompensationAmount = userInquiry.CompensationAmount,
            TrackingUnitType = userInquiry.InquiryTemplate.TrackingUnitType,
            HistoricalRemainingDays = userInquiry.HistoricalRemainingDays,
            HistoricalBalanceDays = userInquiry.HistoricalBalanceDays,
            HistoricalBalanceHours = userInquiry.HistoricalBalanceHours,
            CreatedById = userInquiry.CreatedById,
            CreatedAtUtc = userInquiry.CreatedAtUtc,
            UpdatedById = userInquiry.UpdatedById,
            UpdatedAtUtc = userInquiry.UpdatedAtUtc,
            InquiryType = userInquiry.InquiryTemplate.InquiryType.ToString(),
            CountryId = userInquiry.CountryId,            

            CreatedByUser = new InquiriesUserDto
            {
                Id = userInquiry.CreatedByUser!.UserId,
                FirstName = userInquiry.CreatedByUser.FirstName,
                LastName = userInquiry.CreatedByUser.LastName,
                Email = userInquiry.CreatedByUser.Email,
                DepartmentId = userInquiry.CreatedByUser.DepartmentId,
                ResourceGroupId = userInquiry.CreatedByUser.ResourceGroupId,
                RegisteredLocationId = userInquiry.CreatedByUser.RegisteredLocationId,
                AvatarFileId = userInquiry.CreatedByUser.AvatarFileId,
                Position = userInquiry.HistoricalCreatedByUserPosition ?? userInquiry.CreatedByUser.Position,
                Color = userInquiry.CreatedByUser.Color
            },
            CreatedForUser = new InquiriesUserDto
            {
                Id = userInquiry.User!.UserId,
                FirstName = userInquiry.User.FirstName,
                LastName = userInquiry.User.LastName,
                Email = userInquiry.User.Email,
                DepartmentId = userInquiry.User.DepartmentId,
                ResourceGroupId = userInquiry.User.ResourceGroupId,
                RegisteredLocationId = userInquiry.User.RegisteredLocationId,
                AvatarFileId = userInquiry.User.AvatarFileId,
                Position = userInquiry.HistoricalUserPosition ?? userInquiry.User.Position,
                Color = userInquiry.User.Color
            },
            ManagerOfUser = userInquiry.HistoricalManagerId == null
                ? null
                : new InquiriesUserDto
                {
                    Id = userInquiry.HistoricalManagerId!.Value,
                    FirstName = userInquiry.HistoricalManager!.FirstName,
                    LastName = userInquiry.HistoricalManager.LastName,
                    Email = userInquiry.HistoricalManager.Email,
                    DepartmentId = userInquiry.HistoricalManager.DepartmentId,
                    ResourceGroupId = userInquiry.HistoricalManager.ResourceGroupId,
                    RegisteredLocationId = userInquiry.HistoricalManager.RegisteredLocationId,
                    AvatarFileId = userInquiry.HistoricalManager.AvatarFileId,
                    Color = userInquiry.HistoricalManager.Color,
                    Position = userInquiry.HistoricalManagerPosition
                },

            UserInquiryFiles = userInquiry.UserInquiryFiles
                .Select(f => new UserInquiryFileCoreDto
                {
                    Id = f.Id,
                    FileId = f.FileId,
                    FileName = f.FileName,
                    FileSize = f.FileSize
                }).ToList(),
            UserInquiryDates = userInquiry.UserInquiryDates
                .Select(d => new UserInquiryDateCoreDto
                {
                    Id = d.Id,
                    Date = d.Date,
                    RequestedDays = d.RequestedDays,
                    DateType = d.DateType,
                    IsExcludedFromInquiry = d.CalendarExclusionId != null,
                    ExcludedDayName = d.CalendarExclusion != null ? d.CalendarExclusion.Name : null
                }).ToList(),
            UserInquiryAccepts = userInquiry.UserInquiryAccepts
                .Select(a => new UserInquiryAcceptCoreDto
                {
                    Id = a.Id,
                    Status = a.Status,
                    AcceptUserPosition = a.AcceptUserPosition,
                    AcceptPolicyScope = a.AcceptPolicyScope,
                    AcceptUser = a.AcceptUser == null
                    ? null
                    : new InquiriesUserDto
                    {
                        Id = a.AcceptUser.UserId,
                        FirstName = a.AcceptUser.FirstName,
                        LastName = a.AcceptUser.LastName,
                        Email = a.AcceptUser.Email,
                        DepartmentId = a.AcceptUser.DepartmentId,
                        AvatarFileId = a.AcceptUser.AvatarFileId,
                        Position = a.AcceptUser.Position,
                        Color = a.AcceptUser.Color
                    },
                    AcceptedByUser = a.AcceptedByUser == null
                    ? null
                    : new InquiriesUserDto
                    {
                        Id = a.AcceptedByUser.UserId,
                        FirstName = a.AcceptedByUser.FirstName,
                        LastName = a.AcceptedByUser.LastName,
                        Email = a.AcceptedByUser.Email,
                        DepartmentId = a.AcceptedByUser.DepartmentId,
                        AvatarFileId = a.AcceptedByUser.AvatarFileId,
                        Position = a.AcceptedByUser.Position,
                        Color = a.AcceptedByUser.Color
                    },
                    AcceptedAtUtc = a.AcceptedAtUtc,
                    RejectedByUser = a.RejectedByUser == null
                    ? null
                    : new InquiriesUserDto
                    {
                        Id = a.RejectedByUser.UserId,
                        FirstName = a.RejectedByUser.FirstName,
                        LastName = a.RejectedByUser.LastName,
                        Email = a.RejectedByUser.Email,
                        DepartmentId = a.RejectedByUser.DepartmentId,
                        AvatarFileId = a.RejectedByUser.AvatarFileId,
                        Position = a.RejectedByUser.Position,
                        Color = a.RejectedByUser.Color
                    },
                    RejectedAtUtc = a.RejectedAtUtc
                })
                .OrderBy(x => x.Id)
                .ToList()
        };
}
