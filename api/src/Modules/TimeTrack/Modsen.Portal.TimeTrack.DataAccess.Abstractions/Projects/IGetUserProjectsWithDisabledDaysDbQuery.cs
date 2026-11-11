using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IGetUserProjectsWithDisabledDaysDbQuery : IDbQuery<GetUserProjectsWithDisabledDaysDbRequest, List<ProjectWithDisabledDays>>;

internal record GetUserProjectsWithDisabledDaysDbRequest(
    int UserId,
    bool? OnlyManagedProjects,
    int CurrentUserId,
    DateOnly TrackIntervalFrom,
    DateOnly TrackIntervalTo);

internal class ProjectWithDisabledDays
{
    public ProjectLookup? ProjectLookup { get; set; }
    public IReadOnlyCollection<DateOnly> DisabledDates { get; set; } = [];
    public IReadOnlyCollection<BillableDate> BillableDates { get; set; } = [];
}

internal class BillableDate
{
    public bool IsBillable { get; set; }
    public DateOnly Date { get; set; }
}
