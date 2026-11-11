using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests;

internal sealed class UpdateExclusionRequest : ICommandHandler<UpdateExclusionRequest.Command, CalendarExclusionDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly ICalendarExclusionValidator _calendarExclusionValidator;

    public UpdateExclusionRequest(IInquiriesDbContext inquiriesDbContext,
        ICalendarExclusionValidator calendarExclusionValidator)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _calendarExclusionValidator = calendarExclusionValidator;
    }

    public async Task<CalendarExclusionDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var exclusion = await _inquiriesDbContext.CalendarExclusions
                .Where(exclusion => exclusion.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken) ??
            throw new EntityNotFoundException($"Calendar exclusion {request.Id} couldn't be found.");

        exclusion.Name = request.Name;
        exclusion.DateType = request.DateType;
        exclusion.Date = request.Date;
        exclusion.NewDate = request.NewDate;
        exclusion.CountryId = request.CountryId;

        await _calendarExclusionValidator.Validate(exclusion, cancellationToken);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return new CalendarExclusionDto
        {
            Id = exclusion.Id,
            Name = exclusion.Name,
            Date = exclusion.Date,
            NewDate = exclusion.NewDate,
            DateType = exclusion.DateType,
            CountryId = exclusion.CountryId,
        };
    }

    internal sealed record Command(int Id) : ICommand<CalendarExclusionDto>, ITransactionalRequest
    {
        public string? Name { get; init; }
        public DateType DateType { get; init; }
        public DateOnly Date { get; set; }
        public DateOnly? NewDate { get; init; }
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
