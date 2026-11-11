using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests;

internal sealed class GetUsersByTemplateLookupRequest : IQueryHandler<GetUsersByTemplateLookupRequest.Query, TemplateUsersLookupDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetUsersByTemplateLookupRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<TemplateUsersLookupDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var inquiryTemplate = await _inquiriesDbContext.InquiryTemplates
            .Include(template => template.InquiryTemplateAgreements)
            .FirstOrDefaultAsync(template =>
                template.Id == request.InquiryTemplateId && template.IsActive,
                cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.InquiryTemplateNotFound);

        var isInquiryTypeWithAgreementsAllowed = UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(inquiryTemplate.InquiryType);

        if (isInquiryTypeWithAgreementsAllowed && !request.AgreementTemplateId.HasValue)
        {
            throw new InvalidDataAppException("Agreement is required for this absence type");
        }

        InquiryTemplateAgreementEntity? agreementTemplate = null;

        if (request.AgreementTemplateId.HasValue)
        {
            agreementTemplate = await _inquiriesDbContext.InquiryTemplateAgreements
                .Where(x => !x.IsDeleted)
                .Include(x => x.InquiryTemplate)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.AgreementTemplateId && x.InquiryTemplate!.IsActive,
                    cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.AgreementTemplateNotFound);

            if (agreementTemplate.InquiryTemplateId != request.InquiryTemplateId)
            {
                throw new EntityNotFoundException($"Agreement with Id = {request.AgreementTemplateId} " +
                    $"is not belong to inquiry with Id = {request.InquiryTemplateId}");
            }
        }

        var assignedUsers = await GetAssignedUsers(request, inquiryTemplate, agreementTemplate, cancellationToken);
        var unassignedUsers = await GetUnAssignedUsers(assignedUsers, request.FullName, cancellationToken);

        return new TemplateUsersLookupDto
        {
            AssignedUsers = assignedUsers,
            UnassignedUsers = unassignedUsers
        };
    }

    private async Task<List<UserLookupDto>> GetAssignedUsers(
        Query request,
        InquiryTemplateEntity inquiryTemplate,
        InquiryTemplateAgreementEntity? agreementTemplate,
        CancellationToken cancellationToken)
    {
        var assignedUsersQuery = GetAssignedUsersQuery(request, inquiryTemplate, agreementTemplate);
        return await LoadUsers(assignedUsersQuery, request.FullName, cancellationToken);
    }

    private Task<List<UserLookupDto>> GetUnAssignedUsers(List<UserLookupDto> assignedUsers, string? fullNameFilter,
        CancellationToken cancellationToken)
    {
        var assignedUserIds = assignedUsers.Select(x => x.Id).ToArray();
        var unassignedUsersQuery = _inquiriesDbContext.InquiriesUsers
            .Where(x => x.IsActive)
            .Where(u => !assignedUserIds.Contains(u.UserId));

        return LoadUsers(unassignedUsersQuery, fullNameFilter, cancellationToken);
    }

    private static async Task<List<UserLookupDto>> LoadUsers(IQueryable<InquiriesUserEntity> query, string? fullNameFilter,
        CancellationToken cancellationToken)
    {
        var detailsQuery = query.Select(x => new UserDetails
        {
            Id = x.UserId,
            FullName = (x.LastNameRu != null && x.FirstNameRu != null)
                ? x.LastNameRu + " " + x.FirstNameRu +
                " (" + x.LastName + " " + x.FirstName + ")"
                : x.LastName + " " + x.FirstName,
            FirstName = x.FirstName,
            LastName = x.LastName
        });

        if (!fullNameFilter.IsNullOrWhiteSpace())
        {
            var searchPattern = fullNameFilter.ToUpper(CultureInfo.InvariantCulture);
            detailsQuery = detailsQuery.Where(u => u.FullName.ToUpper().Contains(searchPattern));
        }

        return await detailsQuery
            .OrderBy(x => x.Id)
            .Select(u => new UserLookupDto(u.Id, u.FullName, u.FirstName, u.LastName))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<InquiriesUserEntity> GetAssignedUsersQuery(
        Query request,
        InquiryTemplateEntity inquiryTemplate,
        InquiryTemplateAgreementEntity? agreementTemplate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (agreementTemplate != null && UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(inquiryTemplate.InquiryType))
        {
            return _inquiriesDbContext.UserAgreements
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .Where(x => x.User!.IsActive)
                .Where(agreement => agreement.InquiryAgreementTemplateId == agreementTemplate.Id)
                .Where(agreement => agreement.DateTo == null)
                .Select(x => x.User!);
        }

        return _inquiriesDbContext.UserInquiryTemplates
            .Where(x => x.AssignedToUser!.IsActive)
            .Where(x => x.InquiryTemplateId == request.InquiryTemplateId && (x.DateTo == null || x.DateTo >= today))
            .Select(x => x.AssignedToUser!);
    }

    internal sealed record Query(int InquiryTemplateId, int? AgreementTemplateId, string? FullName) : IQuery<TemplateUsersLookupDto>;

    internal class UserDetails
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
