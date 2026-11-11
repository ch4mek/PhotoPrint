using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;

internal interface IInquiryTemplateLoader
{
    Task<InquiryTemplateDto> LoadAsync(int id, CancellationToken cancellationToken);
    Task<List<InquiryTemplateDto>> LoadAsync(int[]? templateIds, CancellationToken cancellationToken);
    Task<List<InquiryTemplateDto>> LoadAllAsync(CancellationToken cancellationToken);
}

internal class InquiryTemplateLoader : IInquiryTemplateLoader
{
    private readonly IInquiriesDbContext _dbContext;
    private readonly IClockService _clockService;

    public InquiryTemplateLoader(
        IInquiriesDbContext dbContext,
        IClockService clockService)
    {
        _dbContext = dbContext;
        _clockService = clockService;
    }

    public async Task<InquiryTemplateDto> LoadAsync(int id, CancellationToken cancellationToken)
    {
        var templates = await LoadAsync([id], cancellationToken);

        return templates.SingleOrDefault() ??
            throw new EntityNotFoundException($"Inquiry template with Id: {id} couldn't be found.");
    }

    public async Task<List<InquiryTemplateDto>> LoadAsync(int[]? templateIds, CancellationToken cancellationToken)
    {
        var templates = await _dbContext.InquiryTemplates
           .Where(x => templateIds == null || templateIds.Contains(x.Id))
           .Where(x => x.IsActive)
           .Include(x => x.InquiryTemplateAssigns)
           .Include(x => x.UserInquiryAgreementAssigns)
           .Include(x => x.InquiryTemplateAgreements)
               .ThenInclude(x => x.UserAgreements)
           .Include(x => x.InquiryTemplateAcceptPolicyPolicies)
               .ThenInclude(x => x.User)
           .AsSplitQuery()
           .Select(x => new InquiryTemplateDto
           {
                Id = x.Id,
                Name = x.Name,
                Type = x.InquiryType,
                ActivityType = x.ActivityType,
                Comment = x.Comment,
                ProhibitOnProbationPeriod = x.ProhibitOnProbationPeriod,
                TrackingUnit = x.TrackingUnitType,
                IsRequireAttachments = x.RequireAttachments,
                IsIncludeHolidays = x.IncludeHolidays,
                IsIncludeWeekends = x.IncludeWeekends,
                CreatedAt = x.CreatedAtUtc,
                AccrualType = x.AccrualType,
                OrderWeight = x.InquiryTypeOrderWeight,
                TransferType = x.IsStaked != null ? (x.IsStaked.Value ? TransferType.Transferred : TransferType.Reset) : null,
                IsSubstitutesToApproveAllowed = x.InquiryTemplateAcceptPolicyPolicies
                    .Any(inquiry => inquiry.SubstituteScope != null),
               Agreements = x.InquiryTemplateAgreements
                    .Where(x => !x.IsDeleted)
                    .Select(agreement => new InquiryAgreementTemplateDto
               {
                    Id = agreement.Id,
                    Name = agreement.Name,
                    DaysPerContractYear = agreement.DaysPerContractYear,
                    IsDefault = agreement.IsDefault,
                    HasAnyTransactions = agreement.UserInquiryAgreementAssigns.Count != 0
               })
               .OrderByDescending(f => f.Id)
               .ToList(),
               AcceptPolicies = x.InquiryTemplateAcceptPolicyPolicies
                   .Select(policy => new InquiryTemplateAcceptPolicyDto
                   {
                       Id = policy.Id,
                       PolicyScope = policy.Scope,
                       User = policy.User == null
                           ? null
                           : new InquiriesUserDto
                           {
                               Id = policy.User.UserId,
                               FirstName = policy.User.FirstName,
                               LastName = policy.User.LastName,
                               Email = policy.User.Email,
                               DepartmentId = policy.User.DepartmentId,
                               RegisteredLocationId = policy.User.RegisteredLocationId,
                           },
                       UserId = policy.User == null
                           ? null
                           : policy.UserId,
                       UserFullName = policy.User == null
                           ? null
                           : $"{policy.User.LastName} {policy.User.FirstName}",
                       OrderWeight = policy.OrderWeight,
                       SubstituteScope = policy.SubstituteScope,
                       SubstituteUserId = policy.SubstituteUser == null
                           ? null
                           : policy.SubstituteUserId,
                       SubstituteUserFullName = policy.SubstituteUser == null
                           ? null
                           : $"{policy.SubstituteUser.LastName} {policy.SubstituteUser.FirstName}",
                   })
                   .OrderBy(f => f.Id)
                   .ToList(),
               HasAnyTransactions = x.UserInquiryAgreementAssigns.Count != 0
           })
           .OrderBy(x => x.OrderWeight)
            .ThenByDescending(x => x.Id)
           .ToListAsync(cancellationToken);

        await PopulateHasAnyTransactionsFlagsAsync(templates, cancellationToken);
        await PopulateAssignedUsersCountsAsync(templates, cancellationToken);

        return templates;
    }

    private async Task PopulateHasAnyTransactionsFlagsAsync(
        List<InquiryTemplateDto> templates,
        CancellationToken cancellationToken)
    {
        if (templates.Count == 0)
        {
            return;
        }

        var templateIds = templates.Select(x => x.Id).Distinct().ToArray();

        var templateIdsWithTransactions = await _dbContext.UserAgreementTransactions
            .AsNoTracking()
            .Where(x => templateIds.Contains(x.InquiryTemplateId))
            .Select(x => x.InquiryTemplateId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (templateIdsWithTransactions.Length == 0)
        {
            return;
        }

        var templateIdsWithTransactionsSet = templateIdsWithTransactions.ToHashSet();

        foreach (var template in templates)
        {
            if (!templateIdsWithTransactionsSet.Contains(template.Id))
            {
                continue;
            }

            template.HasAnyTransactions = true;
            foreach (var agreement in template.Agreements)
            {
                agreement.HasAnyTransactions = true;
            }
        }
    }

    private async Task PopulateAssignedUsersCountsAsync(
        List<InquiryTemplateDto> templates,
        CancellationToken cancellationToken)
    {
        foreach (var template in templates)
        {
            template.AssignedUsersCount = await GetTemplateAssignedUsersCountAsync(
                template.Id,
                template.Type,
                cancellationToken);

            await PopulateAgreementAssignedUsersCountsAsync(template.Agreements, cancellationToken);
        }
    }

    private async Task<int> GetTemplateAssignedUsersCountAsync(
        int templateId,
        InquiryType templateType,
        CancellationToken cancellationToken)
    {
        var isAgreementBased = UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(templateType);

        if (isAgreementBased)
        {
            return await _dbContext.InquiryTemplateAgreements
                .Where(x => !x.IsDeleted)
                .Where(agreementTemplate => agreementTemplate.InquiryTemplateId == templateId)
                .SelectMany(agreementTemplate => agreementTemplate.UserAgreements.Where(x => !x.IsDeleted))
                .Include(agreement => agreement.User)
                .Where(agreement => agreement.User!.IsActive
                    && (agreement.DateTo == null || _clockService.TodayUtc() <= agreement.DateTo))
                .Select(agreement => agreement.UserId)
                .Distinct()
                .CountAsync(cancellationToken);
        }

        return await _dbContext.UserInquiryTemplates
            .Where(assign => assign.InquiryTemplateId == templateId)
            .CountAsync(assign => assign.AssignedToUser!.IsActive
                && (assign.DateTo == null || _clockService.TodayUtc() <= assign.DateTo),
                cancellationToken);
    }

    private async Task PopulateAgreementAssignedUsersCountsAsync(
        IReadOnlyCollection<InquiryAgreementTemplateDto> agreements,
        CancellationToken cancellationToken)
    {
        foreach (var agreementTemplate in agreements)
        {
            agreementTemplate.AssignedUsersCount = await GetAgreementTemplateAssignedUsersCountAsync(
                agreementTemplate.Id,
                cancellationToken);
        }
    }

    private async Task<int> GetAgreementTemplateAssignedUsersCountAsync(
        int agreementTemplateId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .Include(agreement => agreement.User)
            .Where(agreement => agreement.InquiryAgreementTemplateId == agreementTemplateId)
            .Where(agreementAssign => agreementAssign.User!.IsActive
                && (agreementAssign.DateTo == null || _clockService.TodayUtc() <= agreementAssign.DateTo))
            .Select(agreement => agreement.UserId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public Task<List<InquiryTemplateDto>> LoadAllAsync(CancellationToken cancellationToken)
    {
        return LoadAsync(null, cancellationToken);
    }
}
