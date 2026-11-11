using FluentValidation;

namespace Modsen.Portal.TeamWork.UseCases.GetDepartmentSnapshotByYear;

internal sealed class GetDepartmentSnapshotByYearQueryValidator : AbstractValidator<GetDepartmentSnapshotByYearQuery>
{
    public GetDepartmentSnapshotByYearQueryValidator()
    {
        RuleFor(x => x.StartYear)
            .GreaterThan(0);

        RuleFor(x => x.EndYear)
            .GreaterThan(x => x.StartYear);
    }
}
