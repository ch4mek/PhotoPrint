using FluentValidation;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests;

internal sealed class CreateExclusionRequest : ICommandHandler<CreateExclusionRequest.Command, CalendarExclusionDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly ICalendarExclusionValidator _calendarExclusionValidator;

    public CreateExclusionRequest(IInquiriesDbContext inquiriesDbContext, ICalendarExclusionValidator calendarExclusionValidator)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _calendarExclusionValidator = calendarExclusionValidator;
    }

    public async Task<CalendarExclusionDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var exclusion = new CalendarExclusionEntity
        {
            Name = request.Name,
            Date = request.Date,
            NewDate = request.NewDate,
            DateType = request.DateType,
            CountryId = request.CountryId,
        };

        var created = _inquiriesDbContext.CalendarExclusions.Add(exclusion);

        await _calendarExclusionValidator.Validate(exclusion, cancellationToken);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return new CalendarExclusionDto
        {
            Id = created.Entity.Id,
            Name = exclusion.Name,
            Date = exclusion.Date,
            NewDate = exclusion.NewDate,
            DateType = exclusion.DateType,
            CountryId = exclusion.CountryId,
        };
    }

    internal sealed record Command : ICommand<CalendarExclusionDto>, ITransactionalRequest
    {
        public string? Name { get; init; }
        public DateOnly Date { get; init; }
        public DateOnly? NewDate { get; init; }
        public DateType DateType { get; init; }
        public int CountryId { get; init; }
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.CountryId)
                .NotNull();

            RuleFor(command => command.DateType)
                .Must(BeValidDateType);

            RuleFor(command => command.Date)
                .NotNull();

            When(command => command.DateType == DateType.Holiday, () =>
            {
                RuleFor(command => command.Name)
                    .NotNull()
                    .NotEmpty()
                    .ShortTextType()
                    .NotOnlyPunctuationMarks();

                RuleFor(command => command.NewDate)
                    .Null();
            });

            When(command => command.DateType == DateType.DayOff, () =>
            {
                RuleFor(command => command.Name)
                    .Null();

                RuleFor(command => command.NewDate)
                    .NotNull();
            });
        }

        private static bool BeValidDateType(DateType dateType)
        {
            return dateType is DateType.Holiday or DateType.DayOff;
        }
    }
}
