using FluentValidation;

namespace Modsen.Portal.TeamWork.UseCases.GetSnapshotByYear;

internal sealed class GetSnapshotByYearQueryValidator : AbstractValidator<GetSnapshotByYearQuery>
{
    public GetSnapshotByYearQueryValidator()
    {
        RuleFor(x => x.StartYear)
            .GreaterThan(0);

        RuleFor(x => x.EndYear)
            .GreaterThan(x => x.StartYear);
    }
}
