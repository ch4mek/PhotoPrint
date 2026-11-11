using System.Linq.Expressions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto
{
    internal class InquiryCoreDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string UserFirstName { get; set; }
        public required string UserLastName { get; set; }
        public required string UserFullName { get; set; }
        public string? UserFirstNameRu { get; set; }
        public string? UserLastNameRu { get; set; }
        public string? UserPosition { get; set; }
        public bool IsUserActive { get; set; }
        public int UserPositionOrderWeight { get; set; }
        public int? UserResourceGroupId { get; set; }
        public string? UserDepartmentId { get; set; }
        public int? AgreementTemplateId { get; set; }
        public int InquiryTemplateId { get; set; }
        public string? InquiryTemplateName { get; set; }
        public bool IsAgreement { get; set; }
        public string? InquiryType { get; set; }
        public int InquiryTypeOrderWeight { get; set; }
        public bool ManagedByRoleRequest { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public decimal? TotalDays { get; set; }
        public decimal? TotalHours { get; set; }
        public decimal? TotalAccountantDays { get; set; }
        public string? Status { get; set; }
        public int StatusOrderWeight { get; set; }
        public string? Notes { get; set; }
        public int? CurrentApproverId { get; set; }
        public int? CreatedById { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public List<UserInquiryAcceptCoreDto> UserInquiryAccepts { get; set; } = [];

        public static readonly Expression<Func<UserInquiryEntity, InquiryCoreDto>> Map =
        userInquiry => new InquiryCoreDto
        {
            Id = userInquiry.Id,
            UserId = userInquiry.UserId,

            UserFirstName = userInquiry.User!.FirstName,
            UserLastName = userInquiry.User!.LastName,
            UserFullName = userInquiry.User!.LastNameRu == null && userInquiry.User!.FirstNameRu == null
                ? userInquiry.User!.LastName + " " + userInquiry.User!.FirstName
                : userInquiry.User!.LastNameRu != null && userInquiry.User!.FirstNameRu != null
                    ? userInquiry.User!.LastNameRu + " " + userInquiry.User!.FirstNameRu + " (" + userInquiry.User!.LastName + " " + userInquiry.User!.FirstName + ")"
                    : (userInquiry.User!.LastNameRu ?? userInquiry.User!.FirstNameRu) + " (" + userInquiry.User!.LastName + " " + userInquiry.User!.FirstName + ")",
            UserFirstNameRu = userInquiry.User!.FirstNameRu,
            UserLastNameRu = userInquiry.User!.LastNameRu,

            UserPosition = userInquiry.User!.Position,
            UserPositionOrderWeight = userInquiry.User!.PositionOrderWeight ?? 0,
            UserResourceGroupId = userInquiry.User!.ResourceGroupId,
            UserDepartmentId = userInquiry.User!.DepartmentId,
            IsUserActive = userInquiry.User!.IsActive,
            AgreementTemplateId = userInquiry.AgreementTemplateId,
            InquiryTemplateId = userInquiry.InquiryTemplateId,
            InquiryTemplateName = userInquiry.InquiryTemplate!.Name,
            IsAgreement = userInquiry.AgreementTemplateId != null,
            InquiryType = userInquiry.InquiryTemplate!.InquiryType.ToString(),
            InquiryTypeOrderWeight = userInquiry.InquiryTemplate!.InquiryTypeOrderWeight,
            ManagedByRoleRequest = false,
            DateFrom = userInquiry.DateFrom,
            DateTo = userInquiry.DateTo,
            TotalDays = userInquiry.TotalDays,
            TotalHours = userInquiry.TotalHours,
            TotalAccountantDays = userInquiry.TotalAccountantDays,
            Status = userInquiry.Status.ToString(),
            StatusOrderWeight = userInquiry.StatusOrderWeight,
            Notes = userInquiry.Notes,
            CurrentApproverId = userInquiry.UserInquiryAccepts
                        .Where(accept => accept.Status == UserInquiryAcceptStatus.WaitingForApprove)
                        .Select(accept => accept.AcceptUserId)
                        .FirstOrDefault(),
            CreatedById = userInquiry.CreatedById,
            CreatedAtUtc = userInquiry.CreatedAtUtc,
            UserInquiryAccepts = userInquiry.UserInquiryAccepts
                .Select(a => new UserInquiryAcceptCoreDto
                {
                    Id = a.Id,
                    Status = a.Status,
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
}
