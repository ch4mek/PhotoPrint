using FluentValidation;
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
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Validators;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;

internal sealed class CreateTemplateRequest : ICommandHandler<CreateTemplateRequest.Command, InquiryTemplateDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IInquiryTemplateLoader _inquiryTemplateLoader;
    private readonly IInquiryTemplateValidator _inquiryTemplateValidator;
    private readonly IAreAcceptPoliciesValidBusinessRule _acceptPoliciesValidBusinessRule;
    private readonly ICommunicationBus _communicationBus;

    public CreateTemplateRequest(IInquiriesDbContext inquiriesDbContext,
        IInquiryTemplateLoader inquiryTemplateLoader,
        IInquiryTemplateValidator inquiryTemplateValidator,
        IAreAcceptPoliciesValidBusinessRule acceptPoliciesValidBusinessRule,
        ICommunicationBus communicationBus
        )
    {
        _inquiriesDbContext = inquiriesDbContext;
        _inquiryTemplateLoader = inquiryTemplateLoader;
        _inquiryTemplateValidator = inquiryTemplateValidator;
        _acceptPoliciesValidBusinessRule = acceptPoliciesValidBusinessRule;
        _communicationBus = communicationBus;
    }

    public async Task<InquiryTemplateDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var template = new InquiryTemplateEntity
        {
            Name = request.Name,
            InquiryType = request.Type,
            IsActive = true,
            ActivityType = request.ActivityType,
            AccrualType = request.AccrualType,
            IsStaked = request.TransferType != null ?  request.TransferType == TransferType.Transferred : null,
            TrackingUnitType = request.TrackingUnit,
            Comment = request.Comment,
            ProhibitOnProbationPeriod = request.IsProhibitOnProbationPeriod,
            RequireAttachments = request.IsRequireAttachments,
            IncludeHolidays = request.IsIncludeHolidays,
            IncludeWeekends = request.IsIncludeWeekends,
        };
        var created = _inquiriesDbContext.InquiryTemplates.Add(template);

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

        var templatePolicyEntities = request.AcceptPolicies
            .Select(policy => new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplate = created.Entity,
                OrderWeight = policy.OrderWeight,
                Scope = policy.Scope,
                UserId = policy.UserId,
                SubstituteScope = policy.SubstituteScope,
                SubstituteUserId = policy.SubstituteUserId
            });
        _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .AddRange(templatePolicyEntities);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

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

        return await _inquiryTemplateLoader.LoadAsync(created.Entity.Id, cancellationToken);
    }

    internal sealed record Command : ICommand<InquiryTemplateDto>, ITransactionalRequest
    {
        public required string Name { get; init; }
        public InquiryType Type { get; init; }
        public TrackingUnit TrackingUnit { get; init; }
        public ActivityType ActivityType { get; init; }
        public TransferType? TransferType { get; set; }
        public AccrualType? AccrualType { get; set; }
        public string? Comment { get; init; }
        public bool IsProhibitOnProbationPeriod { get; init; }
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

            RuleFor(x => x.Comment)
                .LongTextType1(false);

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

            RuleFor(x => x.TransferType);
            RuleFor(x => x.AccrualType);

            RuleFor(command => command.IsSubstitutesToApproveAllowed)
                .NotNull();

            RuleForEach(command => command.AcceptPolicies)
                .SetValidator(new AcceptPolicyValidator());
        }
    }
}
