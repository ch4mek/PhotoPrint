using FluentValidation;
using Modsen.Portal.Core.UseCases.ValidationRules;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal class UpdateTaskNameDto
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public required string OldTaskName { get; set; }
    public required string TaskName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    internal sealed class Validator : AbstractValidator<UpdateTaskNameDto>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User Id should be greater than 0");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0)
                .WithMessage("Project Id should be greater than 0");

            RuleFor(x => x.OldTaskName)
                .BeEmptyOrInLength(2, 250)
                .SpacesRule();

            RuleFor(x => x.TaskName)
                .BeEmptyOrInLength(2, 250)
                .SpacesRule();
        }
    }
}
