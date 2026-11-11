using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class ProjectWithDisabledDaysDto
{
    public ProjectLookupDto? Project { get; set; }
    public IReadOnlyCollection<DateOnly> DisabledDates { get; set; } = [];
    public IReadOnlyCollection<BillableAssignementDate> BillabilityDays { get; set; } = [];

    internal static ProjectWithDisabledDaysDto Of(ProjectWithDisabledDays source, bool isCurrentUserAbleToSeeBillableTrackedTime)
    {
        return new ProjectWithDisabledDaysDto
        {
            Project = ProjectLookupDto.Of(source.ProjectLookup, isCurrentUserAbleToSeeBillableTrackedTime),
            DisabledDates = source.DisabledDates,
            BillabilityDays = source.BillableDates.Select(x => new BillableAssignementDate
            {
                Date = x.Date,
                IsBillable = x.IsBillable
            }).ToArray(),
        };
    }

    internal static ProjectWithDisabledDaysDto Of(ProjectWithDisabledDaysDto source, bool isCurrentUserAbleToSeeBillableTrackedTime)
    {
        return new ProjectWithDisabledDaysDto
        {
            Project = ProjectLookupDto.Of(source.Project, isCurrentUserAbleToSeeBillableTrackedTime),
            DisabledDates = source.DisabledDates,
            BillabilityDays = source.BillabilityDays.Select(x => new BillableAssignementDate
            {
                Date = x.Date,
                IsBillable = x.IsBillable
            }).ToArray(),
        };
    }
}
