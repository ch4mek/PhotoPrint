using FluentValidation;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto
{
    internal sealed record CreatePerformanceReviewDto(
    int UserId,
    DateTime Date)
    {
        internal sealed class Validator : AbstractValidator<CreatePerformanceReviewDto>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotNull();

                RuleFor(x => x.Date)
                .NotNull()
                .Must(x=> x.Kind == DateTimeKind.Utc)
                .WithMessage("DateTime kind must be Utc")
                .GreaterThanOrEqualTo(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            }
        }
    }
}
