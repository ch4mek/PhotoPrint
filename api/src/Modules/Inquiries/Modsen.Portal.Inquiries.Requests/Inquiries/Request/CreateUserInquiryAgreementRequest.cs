using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Builders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Validators;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class CreateUserInquiryAgreementRequest : ICommandHandler<CreateUserInquiryAgreementRequest.Command, UserInquiryCoreDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserInquiryBuilder _userInquiryBuilder;
    private readonly IUserInquiryLoader _userInquiryLoader;
    private readonly INotificationSender _notificationSender;
    private readonly IUserAgreementBuilder _userAgreementBuilder;
    private readonly ICreateUserInquiryAgreementRequestValidator _createUserInquiryAgreementRequestValidator;
    private readonly IIsAgreementValidForInquiryTemplateBusinessRule _isAgreementValidBusinessRule;
    private readonly IInquiryRejectedStatusResolver _inquiryRejectedStatusResolver;
    private readonly INextApproversToNotifyResolver _nextApproversToNotifyResolver;

    public CreateUserInquiryAgreementRequest(
        IInquiriesDbContext inquiriesDbContext,
        IUserInquiryBuilder userInquiryBuilder,
        IUserInquiryLoader userInquiryLoader,
        INotificationSender notificationSender,
        IUserAgreementBuilder userAgreementBuilder,
        ICreateUserInquiryAgreementRequestValidator createUserInquiryAgreementRequestValidator,
        IIsAgreementValidForInquiryTemplateBusinessRule isAgreementValidBusinessRule,
        IInquiryRejectedStatusResolver inquiryRejectedStatusResolver,
        INextApproversToNotifyResolver nextApproversToNotifyResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _userInquiryBuilder = userInquiryBuilder;
        _userInquiryLoader = userInquiryLoader;
        _notificationSender = notificationSender;
        _userAgreementBuilder = userAgreementBuilder;
        _createUserInquiryAgreementRequestValidator = createUserInquiryAgreementRequestValidator;
        _isAgreementValidBusinessRule = isAgreementValidBusinessRule;
        _inquiryRejectedStatusResolver = inquiryRejectedStatusResolver;
        _nextApproversToNotifyResolver = nextApproversToNotifyResolver;
    }

    public async Task<UserInquiryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _inquiriesDbContext.InquiriesUsers
            .Where(x => x.UserId == request.UserId)
            .Where(x => x.IsActive)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var inquiryTemplate = await _inquiriesDbContext.InquiryTemplates
                .FirstOrDefaultAsync(it => it.Id == request.InquiryTemplateId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.InquiryTemplateNotFound);

        await ValidateInquiryTemplateAsync(inquiryTemplate, request, cancellationToken);

        if (request.AgreementTemplateId is not null)
        {
            var agreementTemplate = await _inquiriesDbContext.InquiryTemplateAgreements
                    .Where(x => !x.IsDeleted)
                    .AsNoTracking()
                    .Include(agreement => agreement.InquiryTemplate)
                    .FirstOrDefaultAsync(agreement => agreement.Id == request.AgreementTemplateId, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.AgreementTemplateNotFound);

            await _createUserInquiryAgreementRequestValidator.ValidateAgreementAsync(
                request,
                agreementTemplate,
                inquiryTemplate.InquiryType,
                cancellationToken);

            var userInquiryAgreementAssign = await _userAgreementBuilder.Build(
                new UserAgreementBuilderRequest
                {
                    UserId = user.UserId,
                    InquiryTemplateId = request.InquiryTemplateId,
                    AgreementTemplateId = request.AgreementTemplateId.Value,
                    IncludeHolidays = inquiryTemplate.IncludeHolidays,
                    IncludeWeekends = inquiryTemplate.IncludeWeekends,
                    Notes = request.Notes,
                    Hours = request.Hours,
                    TrackingUnitType = inquiryTemplate.TrackingUnitType,
                    DateFrom = request.DateFrom,
                    DateTo = request.DateTo,
                },
                cancellationToken);

            _inquiriesDbContext.UserInquiries.Add(userInquiryAgreementAssign);
            await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

            var inquiryFiles = request.Files.Select(x => new UserInquiryFileEntity
            {
                FileId = x.FileId,
                FileName = x.FileName,
                FileSize = x.FileSize,
                UserInquiryId = userInquiryAgreementAssign.Id,
            });

            _inquiriesDbContext.UserInquiryFiles.AddRange(inquiryFiles);

            var inquiryAccept = userInquiryAgreementAssign.UserInquiryAccepts.FirstOrDefault();

            return await UpdateInquiry(inquiryAccept, userInquiryAgreementAssign, cancellationToken);
        }
        else
        {
            await _createUserInquiryAgreementRequestValidator.ValidateInquiryAsync(request, inquiryTemplate,
                cancellationToken);

            var userInquiryAgreementAssign = await _userInquiryBuilder.Build(new UserInquiryBuilderRequest
            {
                UserId = user.UserId,
                InquiryTemplateId = request.InquiryTemplateId,
                IncludeHolidays = inquiryTemplate.IncludeHolidays,
                IncludeWeekends = inquiryTemplate.IncludeWeekends,
                Notes = request.Notes,
                Hours = request.Hours,
                TrackingUnitType = inquiryTemplate.TrackingUnitType,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
            }, cancellationToken);

            _inquiriesDbContext.UserInquiries.Add(userInquiryAgreementAssign);
            await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

            var inquiryFiles = request.Files.Select(x => new UserInquiryFileEntity
            {
                FileId = x.FileId,
                FileName = x.FileName,
                FileSize = x.FileSize,
                UserInquiryId = userInquiryAgreementAssign.Id,
            });

            _inquiriesDbContext.UserInquiryFiles.AddRange(inquiryFiles);

            var inquiryAccept = userInquiryAgreementAssign.UserInquiryAccepts.MinBy(x => x.Id);

            return await UpdateInquiry(inquiryAccept, userInquiryAgreementAssign, cancellationToken);
        }
    }

    private async Task<UserInquiryCoreDto> UpdateInquiry(
        UserInquiryAcceptEntity? inquiryAccept,
        UserInquiryEntity userInquiryAgreementAssign,
        CancellationToken cancellationToken)
    {
        if (inquiryAccept == null)
        {
            await _inquiryRejectedStatusResolver.ResolveAsync(userInquiryAgreementAssign, cancellationToken);
            await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

            return await GetUserInquiry(userInquiryAgreementAssign.Id, cancellationToken);
        }

        if (inquiryAccept.AcceptPolicyScope == InquiryAcceptPolicyScopes.ProjectManager)
        {
            foreach (var accept in userInquiryAgreementAssign.UserInquiryAccepts
                .Where(x => x.AcceptPolicyScope == InquiryAcceptPolicyScopes.ProjectManager))
            {
                accept.Status = UserInquiryAcceptStatus.WaitingForApprove;
            }
        }
        else
        {
            inquiryAccept.Status = UserInquiryAcceptStatus.WaitingForApprove;
        }

        var waitingForApproveAcceptEntities = userInquiryAgreementAssign.UserInquiryAccepts
            .Where(x => x.Status == UserInquiryAcceptStatus.WaitingForApprove);

        var approverIdsToNotify = await _nextApproversToNotifyResolver
            .ResolveAsync(waitingForApproveAcceptEntities.ToArray(), userInquiryAgreementAssign, cancellationToken);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        var inquiry = await GetUserInquiry(userInquiryAgreementAssign.Id, cancellationToken);

        await _notificationSender.SendInquiryAcceptNotification(
            userInquiryAgreementAssign.UserId,
            inquiry.Id,
            approverIdsToNotify,
            cancellationToken);

        return inquiry;
    }

    private async Task ValidateInquiryTemplateAsync(InquiryTemplateEntity inquiryTemplate, Command request,
        CancellationToken cancellationToken)
    {
        if (inquiryTemplate.TrackingUnitType == TrackingUnit.Hours && !request.Hours.HasValue)
        {
            throw new InvalidDataAppException("You must include hours to request due to inquiry type");
        }

        var isAgreementValid = await _isAgreementValidBusinessRule.IsCorrespondsToBusiness(inquiryTemplate.InquiryType,
            inquiryTemplate.Id,
            request.AgreementTemplateId,
            cancellationToken);

        if (!isAgreementValid)
        {
            throw new InvalidDataAppException("Wrong agreementId provided.");
        }
    }

    private async Task<UserInquiryCoreDto> GetUserInquiry(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await _userInquiryLoader.Load(inquiryId, cancellationToken);
        return inquiry ?? throw new EntityNotFoundException(PortalMessages.UserInquiryNotFound);
    }

    internal sealed record Command : ICommand<UserInquiryCoreDto>, ITransactionalRequest
    {
        public int UserId { get; set; }
        public int InquiryTemplateId { get; set; }
        public int? AgreementTemplateId { get; set; }
        public int? Hours { get; set; }
        public string? Notes { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public IReadOnlyCollection<CreateInquiryFileDto> Files { get; set; } = [];
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DateTo)
                .GreaterThanOrEqualTo(x => x.DateFrom)
                .When(x => !x.Hours.HasValue);

            RuleFor(x => x.DateTo)
                .LessThanOrEqualTo(x => x.DateFrom.AddYears(1))
                .When(x => !x.Hours.HasValue);

            RuleFor(x => x.DateTo)
                .Equal(x => x.DateFrom)
                .When(x => x.Hours.HasValue);

            RuleFor(x => x.Hours)
                .InclusiveBetween(0, 8)
                .When(x => x.Hours.HasValue);

            RuleFor(x => x.Notes)
                .BeEmptyNullOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();
        }
    }
}
