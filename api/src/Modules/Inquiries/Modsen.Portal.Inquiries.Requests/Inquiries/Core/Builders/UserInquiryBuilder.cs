using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Builders;

internal interface IUserInquiryBuilder
{
    Task<UserInquiryEntity> Build(UserInquiryBuilderRequest request, CancellationToken cancellationToken);
}

internal class UserInquiryBuilder : IUserInquiryBuilder
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IInquiryDatesComposer _inquiryDatesComposer;
    private readonly ITotalAccountantDaysCalculator _totalAccountantDaysCalculator;
    private readonly IInquiryAcceptsComposer _inquiryAcceptsComposer;

    public UserInquiryBuilder(
        IInquiriesDbContext inquiriesDbContext,
        IInquiryDatesComposer inquiryDatesComposer,
        ITotalAccountantDaysCalculator totalAccountantDaysCalculator,
        IInquiryAcceptsComposer inquiryAcceptsComposer)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _inquiryDatesComposer = inquiryDatesComposer;
        _totalAccountantDaysCalculator = totalAccountantDaysCalculator;
        _inquiryAcceptsComposer = inquiryAcceptsComposer;
    }

    public async Task<UserInquiryEntity> Build(UserInquiryBuilderRequest request, CancellationToken cancellationToken)
    {
        var countryId = await GetUserCountryId(request.UserId, cancellationToken);

        var inquiryAcceptEntities = await _inquiryAcceptsComposer.ComposeAsync(request.UserId,
            request.InquiryTemplateId,
            cancellationToken);

        var inquiryTemplate = await _inquiriesDbContext.InquiryTemplates.FirstOrDefaultAsync(
            x => x.Id == request.InquiryTemplateId, cancellationToken);

        var userInquiryDateEntities = new List<UserInquiryDateEntity>();
        decimal? totalDays = null;
        decimal? totalAccountantDays = null;
        decimal? totalHours = null;

        var inquiryDateEntities = await _inquiryDatesComposer.Compose(
            new InquiryDatesComposerRequest
            {
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                CountryId = countryId,
                RequestedDays = 1
            }, cancellationToken);

        var totalAccountantDaysCoreDto = _totalAccountantDaysCalculator.Calculate(
            request.IncludeHolidays,
            request.IncludeWeekends,
            inquiryDateEntities.Select(x => (x.DateType, x.Date)).ToList());

        if (request.TrackingUnitType == TrackingUnit.Days)
        {
            totalDays = totalAccountantDaysCoreDto.TotalDays;
            totalAccountantDays = totalAccountantDaysCoreDto.TotalAccoundantDays;
            userInquiryDateEntities.AddRange(inquiryDateEntities);
        }
        else if (request.TrackingUnitType == TrackingUnit.Hours)
        {
            totalHours = totalAccountantDaysCoreDto.TotalAccoundantDays == 0 ? 0 : request.Hours;
        }

        return new UserInquiryEntity
        {
            UserId = request.UserId,
            InquiryTemplate = inquiryTemplate,
            InquiryTemplateId = request.InquiryTemplateId,
            CountryId = countryId,
            Status = UserInquiryStatus.Open,
            Notes = request.Notes,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            TotalDays = totalDays,
            TotalAccountantDays = totalAccountantDays,
            TotalHours = totalHours,
            UserInquiryDates = userInquiryDateEntities,
            UserInquiryAccepts = inquiryAcceptEntities
        };
    }

    private async Task<int> GetUserCountryId(int userId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.InquiriesUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.RegisteredLocationId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new EntityNotFoundException($"Cannot find Country for user with ID {userId}");
    }
}

internal record UserInquiryBuilderRequest
{
    public int UserId { get; set; }
    public int InquiryTemplateId { get; set; }

    public bool IncludeHolidays { get; set; }
    public bool IncludeWeekends { get; set; }
    public TrackingUnit TrackingUnitType { get; set; }
    public int? Hours { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}
