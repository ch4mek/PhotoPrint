using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class DeleteTemplateRequest : ICommandHandler<DeleteTemplateRequest.Command, InquiryTemplateDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;

    public DeleteTemplateRequest(
        IInquiriesDbContext inquiriesDbContext,
        IClockService clockService)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
    }

    public async Task<InquiryTemplateDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _inquiriesDbContext.InquiryTemplates
            .Include(template => template.InquiryTemplateAgreements)
            .FirstOrDefaultAsync(template => template.Id == request.Id, cancellationToken);

        if (entity is null || !entity.IsActive)
        {
            throw new EntityNotFoundException(PortalMessages.InquiryTemplateNotFound);
        }

        var activeAssignsOnInquiryTemplateCount = await _inquiriesDbContext.UserInquiryTemplates
            .Where(assign =>
                assign.InquiryTemplateId == entity.Id &&
                (assign.DateTo >= _clockService.TodayUtc() || assign.DateTo == null))
            .CountAsync(cancellationToken);

        if (activeAssignsOnInquiryTemplateCount > 0)
        {
            throw new EntityConflictException("You cannot delete absence with assigned users");
        }

        if (UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(entity.InquiryType))
        {
            var inquiryAgreementAssigns = _inquiriesDbContext.InquiryTemplateAgreements
                .Where(x => !x.IsDeleted)
                .Where(agreement => agreement.InquiryTemplateId == request.Id)
                .Include(agreement => agreement.UserAgreements)
                    .ThenInclude(x => x.User)
                .SelectMany(agreement => agreement.UserAgreements)
                .Where(x => !x.IsDeleted);

            var activeAssignsOnAgreementTemplate = inquiryAgreementAssigns
                .Where(x => _clockService.TodayUtc() <= x.DateTo || x.DateTo == null)
                .Where(x => x.User!.IsActive);

            if (await activeAssignsOnAgreementTemplate.AnyAsync(cancellationToken))
            {
                throw new EntityConflictException("You cannot delete absence with assigned users");
            }

            // remove agreements assigns
            await inquiryAgreementAssigns
                .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsDeleted, true), cancellationToken);

            // remove agreements templates
            await _inquiriesDbContext.InquiryTemplateAgreements
                .Where(x => x.InquiryTemplateId == request.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsDeleted, true), cancellationToken);
        }

        entity.IsActive = false;

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return new InquiryTemplateDto
        {
            Id = entity.Id,
            ActivityType = entity.ActivityType,
            Comment = entity.Comment,
            IsIncludeHolidays = entity.IncludeHolidays,
            IsIncludeWeekends = entity.IncludeWeekends,
            Name = entity.Name,
            IsRequireAttachments = entity.RequireAttachments,
            ProhibitOnProbationPeriod = entity.ProhibitOnProbationPeriod,
            TrackingUnit = entity.TrackingUnitType,
            Type = entity.InquiryType
        };
    }

    internal sealed record Command(int Id) : ICommand<InquiryTemplateDto>, ITransactionalRequest;
}
