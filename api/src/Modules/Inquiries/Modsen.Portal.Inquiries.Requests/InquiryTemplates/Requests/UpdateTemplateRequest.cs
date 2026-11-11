using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.Contract.Events;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Validators;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class UpdateTemplateRequest : ICommandHandler<UpdateTemplateRequest.Command, InquiryTemplateDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IInquiryTemplateLoader _inquiryTemplateLoader;
    private readonly IInquiryTemplateValidator _inquiryTemplateValidator;
    private readonly IAreAcceptPoliciesValidBusinessRule _acceptPoliciesValidBusinessRule;
    private readonly IUpdateTemplateAcceptPoliciesResolver _updateTemplateAcceptPoliciesResolver;
    private readonly ICommunicationBus _communicationBus;


    public UpdateTemplateRequest(IInquiriesDbContext inquiriesDbContext,
        IInquiryTemplateLoader inquiryTemplateLoader,
        IInquiryTemplateValidator inquiryTemplateValidator,
        IAreAcceptPoliciesValidBusinessRule acceptPoliciesValidBusinessRule,
        IUpdateTemplateAcceptPoliciesResolver updateTemplateAcceptPoliciesResolver,
        ICommunicationBus communicationBus
    )
    {
        _inquiriesDbContext = inquiriesDbContext;
        _inquiryTemplateLoader = inquiryTemplateLoader;
        _inquiryTemplateValidator = inquiryTemplateValidator;
        _acceptPoliciesValidBusinessRule = acceptPoliciesValidBusinessRule;
        _updateTemplateAcceptPoliciesResolver = updateTemplateAcceptPoliciesResolver;
        _communicationBus = communicationBus;
    }

    public async Task<InquiryTemplateDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var template = await GetTemplateEntity(request.Id, cancellationToken);
        template.Name = request.Name;

        var hasTransactions = await _inquiriesDbContext.UserAgreementTransactions
            .AnyAsync(x => x.InquiryTemplateId == template.Id, cancellationToken);

        if (hasTransactions && IsCoreFieldsChanged(template, request))
        {
            throw new ActionNotAllowedException(
                "Can not edit Absence type, Tracking unit, Accrual type, Transfer type or Activity type if template has any transactions");
        }

        if (!hasTransactions)
        {
            template.InquiryType = request.Type;
            template.TrackingUnitType = request.TrackingUnit;
            template.AccrualType = request.AccrualType;
            template.IsStaked = request.TransferType != null ? request.TransferType == TransferType.Transferred : null;
            template.ActivityType = request.ActivityType;
        }

        template.RequireAttachments = request.IsRequireAttachments;
        template.IncludeHolidays = request.IsIncludeHolidays;
        template.IncludeWeekends = request.IsIncludeWeekends;
        template.Comment = request.Comment;
        template.ProhibitOnProbationPeriod = request.IsProhibitOnProbationPeriod;

        await _inquiryTemplateValidator.Validate(template, cancellationToken);

        var businessRuleResult = await _acceptPoliciesValidBusinessRule
            .IsCorrespondsToBusiness(new AreAcceptPoliciesValidBusinessRule.Request
            {
                IsSubstitutesToApproveAllowed = request.IsSubstitutesToApproveAllowed,
                AcceptPolices = request.AcceptPolicies
            }, cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new InvalidDataAppException(businessRuleResult.Reason ?? "");
        }

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
        await _updateTemplateAcceptPoliciesResolver.ResolveAsync(template, request.AcceptPolicies, cancellationToken);

        var approversIds = request.AcceptPolicies
            .SelectMany(policy => new[] { policy.UserId, policy.SubstituteUserId })
            .OfType<int>()
            .Distinct()
            .ToList();

        var notification = new InquiryTemplateApproversUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ApproversIds = approversIds
        };
        await _communicationBus.PublishAsync(notification, cancellationToken);

        return await _inquiryTemplateLoader.LoadAsync(template.Id, cancellationToken);
    }

    private async Task<InquiryTemplateEntity> GetTemplateEntity(int id, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.InquiryTemplates
            .Include(x => x.InquiryTemplateAcceptPolicyPolicies)
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken) ??
            throw new EntityNotFoundException($"Template by id {id} is not found");
    }

    private static bool IsCoreFieldsChanged(InquiryTemplateEntity template, Command request)
    {
        return template.InquiryType != request.Type ||
            template.TrackingUnitType != request.TrackingUnit ||
            template.AccrualType != request.AccrualType ||
            GetTransferType(template) != request.TransferType ||
            template.ActivityType != request.ActivityType;
    }

    private static TransferType? GetTransferType(InquiryTemplateEntity template)
    {
        if (!template.IsStaked.HasValue)
        {
            return null;
        }

        return template.IsStaked.Value ? TransferType.Transferred : TransferType.Reset;
    }

    internal sealed record Command : ICommand<InquiryTemplateDto>, ITransactionalRequest
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public InquiryType Type { get; init; }
        public TrackingUnit TrackingUnit { get; set; }
        public ActivityType ActivityType { get; set; }
        public AccrualType? AccrualType { get; set; }
        public TransferType? TransferType { get; set; }
        public string? Comment { get; set; }
        public bool IsProhibitOnProbationPeriod { get; set; }
        public bool IsRequireAttachments { get; init; }
        public bool IsIncludeHolidays { get; init; }
        public bool IsIncludeWeekends { get; init; }
        public bool IsSubstitutesToApproveAllowed { get; init; }
        public List<UpdateInquiryTemplateAcceptPolicyDto> AcceptPolicies { get; init; } = [];
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .AlphabeticWithHyphensSpaces();

            RuleFor(x => x.Type)
                .NotEqual(InquiryType.Unknown);

            RuleFor(x => x.TrackingUnit)
                .NotEqual(TrackingUnit.Hours)
                .When(x => UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.Type))
                .WithMessage("TrackingUnit cannot be 'Hours' for Vacation or SickDay.");

            When(x => UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.Type), () =>
            {
                When(x => x.AccrualType == AccrualType.ProportianallyEachMonth, () =>
                {
                    RuleFor(x => x.TransferType)
                    .NotEqual(TransferType.Reset)
                    .WithMessage("TransferType can be reset only for OnceInYear AccrualType");
                });

                RuleFor(x => x.AccrualType)
                .NotNull()
                .WithMessage("AccrualType required for Vacation and SickDay.");

                RuleFor(x => x.TransferType)
                .NotNull()
                .WithMessage("TransferType required for Vacation and SickDay.");
            });

            When(x => !UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.Type), () =>
            {
                RuleFor(x => x.AccrualType)
                .Null()
                .WithMessage("TransferType must be null Vacation and SickDay.");

                RuleFor(x => x.AccrualType)
                .Null()
                .WithMessage("AccrualType must be null Vacation and SickDay.");
            });

            RuleFor(x => x.Comment)
                .LongTextType1(false);

            RuleFor(command => command.IsSubstitutesToApproveAllowed)
                .NotNull();

            RuleForEach(command => command.AcceptPolicies)
                .SetValidator(new AcceptPolicyValidator());
        }
    }

}
