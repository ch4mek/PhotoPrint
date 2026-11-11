using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests;

internal sealed class CreateInquiriesAssignsRequest
    : ICommandHandler<CreateInquiriesAssignsRequest.Command, InquiriesAssignsDto>
{
    private readonly IUserInquiryTemplateComposer _inquiryTemplateComposer;
    private readonly IInquiriesDbContext _dbContext;
    private readonly IChangedAgreementTransactionComposer _changedAgreementTransactionComposer;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IRejectInquiriesAfterUnassignResolver _rejectInquiriesAfterUnassignResolver;
    private readonly IResetUserAgreementBalanceComposer _resetUserAgreementBalanceComposer;

    public CreateInquiriesAssignsRequest(
        IUserInquiryTemplateComposer inquiryTemplateComposer,
        IInquiriesDbContext dbContext,
        IChangedAgreementTransactionComposer changedAgreementTransactionComposer,
        IAuthenticatedUserContext authenticatedUserContext,
        IRejectInquiriesAfterUnassignResolver rejectInquiriesAfterUnassignResolver,
        IResetUserAgreementBalanceComposer resetUserAgreementBalanceComposer)
    {
        _inquiryTemplateComposer = inquiryTemplateComposer;
        _dbContext = dbContext;
        _changedAgreementTransactionComposer = changedAgreementTransactionComposer;
        _authenticatedUserContext = authenticatedUserContext;
        _rejectInquiriesAfterUnassignResolver = rejectInquiriesAfterUnassignResolver;
        _resetUserAgreementBalanceComposer = resetUserAgreementBalanceComposer;
    }

    public async Task<InquiriesAssignsDto> Handle(Command request, CancellationToken cancellationToken)
    {
        ValidateHrCanOnlyUnassign(request);
        await ValidateUsers(request, cancellationToken);

        var inquiryTemplate = await _dbContext.InquiryTemplates
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

        if (request.AgreementTemplateId.HasValue)
        {
            var agreementTemplate = await _dbContext.InquiryTemplateAgreements
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

            var effectiveFrom = request.EffectiveFrom
                ?? throw new InvalidDataAppException("EffectiveFrom is required for this absence type");

            return await CreateUserAgreements(
                inquiryTemplate,
                agreementTemplate,
                effectiveFrom,
                request.AssignedUserIds,
                request.UnassignedUserIds,
                cancellationToken);
        }
        else
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var usersInquiries = await _inquiryTemplateComposer.Compose(
                request.AssignedUserIds,
                today,
                inquiryTemplate.Id,
                cancellationToken);

            var usersTemplates = await _dbContext.UserInquiryTemplates
                .Where(x => x.InquiryTemplateId == request.InquiryTemplateId && request.UnassignedUserIds.Contains(x.AssignedToUserId))
                .ToListAsync(cancellationToken);

            foreach (var template in usersTemplates)
            {
                template.DateTo = today.AddDays(-1);
            }

            await _rejectInquiriesAfterUnassignResolver.ResolveAsync(request.UnassignedUserIds, inquiryTemplate.Id, cancellationToken);

            _dbContext.UserInquiryTemplates.AddRange(usersInquiries);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var assignedUserInquiryIds = usersInquiries.Select(x => x.Id).ToList();

            return new InquiriesAssignsDto(assignedUserInquiryIds);
        }
    }

    private void ValidateHrCanOnlyUnassign(Command request)
    {
        var isOnlyHr = _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name)
            && !_authenticatedUserContext.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name);

        if (isOnlyHr && request.AssignedUserIds.Length > 0)
        {
            throw new ActionNotAllowedException("HR users can only unassign users");
        }
    }

    private async Task ValidateUsers(Command request, CancellationToken cancellationToken)
    {
        var assignUserIds = await _dbContext.InquiriesUsers
            .Where(x => x.IsActive && request.AssignedUserIds.Contains(x.UserId))
            .Select(x => x.UserId)
            .ToArrayAsync(cancellationToken);

        if (assignUserIds.Length != request.AssignedUserIds.Length)
        {
            throw new EntityNotFoundException("Not all users exist");
        }

        var unassignUserIds = await _dbContext.InquiriesUsers
            .Where(x => x.IsActive && request.UnassignedUserIds.Contains(x.UserId))
            .Select(x => x.UserId)
            .ToArrayAsync(cancellationToken);

        if (unassignUserIds.Length != request.UnassignedUserIds.Length)
        {
            throw new EntityNotFoundException("Not all users exist");
        }

        if (unassignUserIds.Intersect(assignUserIds).Any())
        {
            throw new InvalidDataAppException("It's not allowed to assign and unassign the same users at the same time");
        }
    }

    private async Task<InquiriesAssignsDto> CreateUserAgreements(
        InquiryTemplateEntity inquiryTemplate,
        InquiryTemplateAgreementEntity agreementTemplate,
        DateOnly effectiveFrom,
        int[] assignUserIds,
        int[] unassignUserIds,
        CancellationToken cancellationToken)
    {
        var assignedUsersAgreements = new List<UserAgreementEntity>();

        foreach (var userId in assignUserIds)
        {
            var activeUserAgreements = await _dbContext.UserAgreements
                .Where(x => !x.IsDeleted)
                .Where(userAgreement =>
                    userAgreement.UserId == userId &&
                    userAgreement.InquiryTemplateAgreement!.InquiryTemplateId == inquiryTemplate.Id)
                .Where(userAgreement =>
                    userAgreement.DateTo >= effectiveFrom ||
                    userAgreement.DateTo == null)
                .ToListAsync(cancellationToken);

            foreach (var activeUserAgreement in activeUserAgreements)
            {
                activeUserAgreement.DateTo = effectiveFrom.AddDays(-1);
            }

            var inactiveUserAgreements = activeUserAgreements
                .Where(x => x.DateFrom > x.DateTo)
                .ToList();

            var agreementAssign = new UserAgreementEntity
            {
                DateFrom = effectiveFrom,
                UserId = userId,
                InquiryAgreementTemplateId = agreementTemplate.Id
            };

            inactiveUserAgreements.ForEach(x => x.IsDeleted = true);
            assignedUsersAgreements.Add(agreementAssign);
        }

        _dbContext.UserAgreements.AddRange(assignedUsersAgreements);

        var inactiveUserAgreementIds = new List<int>();

        foreach (var userId in unassignUserIds)
        {
            var activeUserAgreements = await _dbContext.UserAgreements
                .Where(x => !x.IsDeleted)
                .Where(userAgreement =>
                    userAgreement.UserId == userId &&
                    userAgreement.InquiryAgreementTemplateId == agreementTemplate.Id)
                .Where(userAgreement =>
                    userAgreement.DateTo >= effectiveFrom ||
                    userAgreement.DateTo == null)
                .ToListAsync(cancellationToken);

            foreach (var activeUserAgreement in activeUserAgreements)
            {
                activeUserAgreement.IsDeleted = true;
            }
        }

        await _rejectInquiriesAfterUnassignResolver.ResolveAsync(unassignUserIds, inquiryTemplate.Id, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await AddCorrectionTransactions(
            effectiveFrom,
            assignUserIds, unassignUserIds,
            inactiveUserAgreementIds,
            inquiryTemplate.Id,
            cancellationToken);

        var assignedUserAgreementIds = assignedUsersAgreements
            .GroupBy(x => x.UserId)
            .Select(g => g.MaxBy(x => x.Id)!.Id)
            .ToList();

        return new InquiriesAssignsDto(assignedUserAgreementIds);
    }

    private async Task AddCorrectionTransactions(
        DateOnly effectiveFrom,
        int[] assignUserIds, int[] unassignUserIds,
        List<int> inactiveUserAgreementIds,
        int inquiryTemplateId,
        CancellationToken cancellationToken)
    {
        foreach (var userId in assignUserIds)
        {
            await _changedAgreementTransactionComposer.Compose(
                userId,
                effectiveFrom,
                null,
                inactiveUserAgreementIds,
                [inquiryTemplateId],
                cancellationToken);
        }

        if (unassignUserIds.Length > 0)
        {
            await _resetUserAgreementBalanceComposer.Compose(
                unassignUserIds,
                inquiryTemplateId,
                cancellationToken);
        }
    }

    internal sealed record Command(
        int[] AssignedUserIds,
        int[] UnassignedUserIds,
        int InquiryTemplateId,
        int? AgreementTemplateId,
        DateOnly? EffectiveFrom) : ICommand<InquiriesAssignsDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator(IClockService clockService)
        {
            RuleFor(command => command.InquiryTemplateId)
                .NotNull();

            RuleFor(command => command.EffectiveFrom)
                .LessThanOrEqualTo(clockService.TodayUtc())
                .When(command => command.EffectiveFrom.HasValue)
                .WithMessage("The effective date must be less than or equal to the current date.");
        }
    }
}
