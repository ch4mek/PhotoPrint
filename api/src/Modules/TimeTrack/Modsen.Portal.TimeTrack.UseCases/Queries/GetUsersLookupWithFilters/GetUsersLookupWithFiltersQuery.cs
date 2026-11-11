using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.GetUsersLookupWithFilters;

internal sealed record GetUsersLookupWithFiltersQuery : IQuery<List<UserDto>>, ITimeTrackRequest
{
    public int? ClientId { get; init; }
    public int? ProjectId { get; init; }
    public string? DepartmentId { get; init; }
    public string? Keyword { get; init; }

    public GetUsersLookupWithFiltersQuery(int? clientId, int? projectId, string? departmentId, string? keyword)
    {
        DepartmentId = departmentId;
        ClientId = clientId;
        ProjectId = projectId;
        Keyword = keyword;
    }
}

