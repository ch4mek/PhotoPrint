using FluentValidation;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

public record CreateSalaryDto(
    int UserId,
    DateOnly Date,
    decimal SalaryUsd,
    int? ConversationId)
{
    internal sealed class Validator : AbstractValidator<CreateSalaryDto>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .WithMessage("This field is required");

            RuleFor(x => x.SalaryUsd)
                .NotNull().NotEmpty()
                .WithMessage("Salary is required.")
                .ExclusiveBetween(0m, 10_000_000m)
                .WithMessage("Salary must be exclusive between 0 and 10,000,000.");

            RuleFor(x => x.Date)
                .GreaterThanOrEqualTo(new DateOnly(2020, 1, 1))
                .WithMessage("Date must be greater than or equal to 01.01.2020.");
        }
    }
}
