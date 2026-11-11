using FluentValidation;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

internal record UpdateSalaryDto(int Id,
    int UserId,
    DateOnly? Date,
    decimal? SalaryUsd)
{
    internal sealed class Validator : AbstractValidator<UpdateSalaryDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("This field is required");

            RuleFor(x => x.UserId)
                .NotNull()
                .WithMessage("This field is required");

            RuleFor(x => x.SalaryUsd)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .When(x => x.SalaryUsd.HasValue)
                .WithMessage("Salary is required if provided.")
                .ExclusiveBetween(0m, 10_000_000m)
                .When(x => x.SalaryUsd.HasValue)
                .WithMessage("Salary must be exclusive between 0 and 10,000,000.");

            RuleFor(x => x.Date)
                .GreaterThanOrEqualTo(new DateOnly(2020, 1, 1))
                .WithMessage("Date must be greater than or equal to 01.01.2020.");
        }
    }
}
