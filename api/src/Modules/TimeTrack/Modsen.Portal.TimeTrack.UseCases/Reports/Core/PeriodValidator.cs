using System.Globalization;
using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Core;

internal class PeriodValidator<T> : AbstractValidator<T>
    where T : IPeriodRequest
{
    public PeriodValidator()
    {
        RuleFor(query => query.MonthOfYear)
            .Must(months => months is null || months.Length == 0)
            .When(query => !query.Year.HasValue)
            .WithMessage(PortalMessages.CannotSpecifyMonthWithoutTheYear);

        RuleFor(query => query.WeekOfYear)
            .Must(week => !week.HasValue)
            .When(query => !query.Year.HasValue)
            .WithMessage(PortalMessages.CannotSpecifyWeekWithoutTheYear);

        RuleFor(query => query.Year)
            .GreaterThanOrEqualTo(DateTime.MinValue.Year)
            .LessThanOrEqualTo(DateTime.MaxValue.Year)
            .DependentRules(() =>
            {
                RuleForEach(query => query.MonthOfYear)
                    .GreaterThanOrEqualTo(DateTime.MinValue.Month)
                    .LessThanOrEqualTo(DateTime.MaxValue.Month)
                    .When(query => query.MonthOfYear is { Length: > 0 });

                RuleFor(query => query.WeekOfYear)
                    .GreaterThanOrEqualTo(1)
                    .LessThanOrEqualTo(query => ISOWeek.GetWeeksInYear(query.Year!.Value))
                    .When(query => query.WeekOfYear.HasValue);
            })
            .When(query => query.Year.HasValue);
    }
}

