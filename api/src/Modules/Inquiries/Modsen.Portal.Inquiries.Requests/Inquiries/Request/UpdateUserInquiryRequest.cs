using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class UpdateUserInquiryRequest : ICommandHandler<UpdateUserInquiryRequest.Command, UserInquiryCoreDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserInquiryLoader _userInquiryLoader;
    private readonly IInquiryDatesComposer _inquiryDatesComposer;
    private readonly ITotalAccountantDaysCalculator _totalAccountantDaysCalculator;
    private readonly IHasUserAccessToMutateInquiryBusinessRule _hasUserAccessToMutateInquiryBusinessRule;

    public UpdateUserInquiryRequest(
        IInquiriesDbContext inquiriesDbContext,
        IUserInquiryLoader userInquiryLoader,
        IInquiryDatesComposer inquiryDatesComposer,
        ITotalAccountantDaysCalculator totalAccountantDaysCalculator,
        IHasUserAccessToMutateInquiryBusinessRule hasUserAccessToMutateInquiryBusinessRule)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _userInquiryLoader = userInquiryLoader;
        _inquiryDatesComposer = inquiryDatesComposer;
        _totalAccountantDaysCalculator = totalAccountantDaysCalculator;
        _hasUserAccessToMutateInquiryBusinessRule = hasUserAccessToMutateInquiryBusinessRule;
    }

    public async Task<UserInquiryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var userInquiry = await _inquiriesDbContext.UserInquiries
            .Include(x => x.UserInquiryDates)
            .Include(x => x.UserInquiryAccepts)
            .Include(x => x.InquiryTemplate)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == request.InquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"User inquiry by id {request.InquiryId} not found");

        if (!await _hasUserAccessToMutateInquiryBusinessRule.IsCorrespondsToBusiness(userInquiry.UserId, cancellationToken))
        {
            throw new ActionNotAllowedException($"You can't update inquiry for user: {userInquiry.UserId}");
        }

        var hasAccepts = userInquiry.UserInquiryAccepts.Any(x =>
        x.Status == UserInquiryAcceptStatus.Accepted
        || x.Status == UserInquiryAcceptStatus.Rejected);

        if (hasAccepts)
        {
            throw new ActionNotAllowedException($"User inquiry by id {request.InquiryId} has reviewed");
        }

        await UpdateInquiryDates(userInquiry, request.DateFrom, request.DateTo, cancellationToken);

        var totalAccoundantDaysCoreDto = _totalAccountantDaysCalculator.Calculate(
            userInquiry.InquiryTemplate!.IncludeHolidays,
            userInquiry.InquiryTemplate.IncludeWeekends,
            userInquiry.UserInquiryDates.Select(x => (x.DateType, x.Date)).ToList());


        userInquiry.TotalDays = totalAccoundantDaysCoreDto.TotalDays;
        userInquiry.TotalAccountantDays = totalAccoundantDaysCoreDto.TotalAccoundantDays;
        userInquiry.Notes = request.Notes;
        userInquiry.DateFrom = request.DateFrom;
        userInquiry.DateTo = request.DateTo;

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return await GetUserInquiry(request.InquiryId, cancellationToken);
    }

    private async Task UpdateInquiryDates(
        UserInquiryEntity userInquiry,
        DateOnly dateFrom,
        DateOnly dateTo,
        CancellationToken cancellationToken)
    {
        var newInquiryDates = await _inquiryDatesComposer.Compose(new InquiryDatesComposerRequest
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CountryId = userInquiry.CountryId,
            RequestedDays = 1
        }, cancellationToken);

        _inquiriesDbContext.UserInquiryDates.RemoveRange(userInquiry.UserInquiryDates);

        userInquiry.UserInquiryDates.Clear();
        userInquiry.UserInquiryDates.AddRange(newInquiryDates);
    }

    private async Task<UserInquiryCoreDto> GetUserInquiry(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await _userInquiryLoader.Load(inquiryId, cancellationToken);
        return inquiry ?? throw new EntityNotFoundException($"Inquiry by id {inquiryId} not found");
    }

    internal sealed record Command : ICommand<UserInquiryCoreDto>, ITransactionalRequest
    {
        public int InquiryId { get; set; }

        public string? Notes { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DateFrom)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

            RuleFor(x => x.DateTo)
                .GreaterThanOrEqualTo(x => x.DateFrom);

            RuleFor(x => x.Notes)
                .BeEmptyNullOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();
        }
    }
}
