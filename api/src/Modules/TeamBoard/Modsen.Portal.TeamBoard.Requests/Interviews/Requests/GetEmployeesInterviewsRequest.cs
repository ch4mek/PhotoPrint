using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests;

internal sealed class GetEmployeesInterviewsRequest (
    IAuthenticatedUserContext authenticatedUserContext,
    ITeamBoardDbContext teamBoardDbContext,
    IClockService clockService)
    : IQueryHandler<GetEmployeesInterviewsRequest.Query, EmployeeInterviewDto[]>
{
    public async Task<EmployeeInterviewDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var userRoles = authenticatedUserContext.GetRoles();
        ValidateUserClaims(request, userRoles);

        var interviews = await GetInterviews(request, userRoles, cancellationToken);
        EnrichResponse(interviews);

        return interviews;
    }

    private static void EnrichResponse(EmployeeInterviewDto[] interviews)
    {
        foreach (var interview in interviews)
        {
            interview.FillFullUserName();
        }
    }

    private void ValidateUserClaims(Query request, IReadOnlyCollection<string> userRoles)
    {
        if (!authenticatedUserContext.IsManager)
        {
            throw new ActionNotAllowedException("Accessing employees' related interviews is allowed only for managers.");
        }

        if (request.DepartmentId.IsNullOrWhiteSpace() ||
            authenticatedUserContext.DepartmentId == request.DepartmentId)
        {
            return;
        }

        if (!userRoles.IsInOneOfTheRoles(PortalRoleNames.Ceo, PortalRoleNames.Cto))
        {
            throw new ActionNotAllowedException("Accessing employees' related interviews from another department is allowed only for CTO/CEO.");
        }
    }

    private Task<EmployeeInterviewDto[]> GetInterviews(
        Query request,
        IReadOnlyCollection<string> userRoles,
        CancellationToken cancellationToken)
    {
        var today = clockService.TodayUtc();
        var canAccessAll = userRoles.IsInOneOfTheRoles(
            PortalRoleNames.Ceo,
            PortalRoleNames.Cto);

        var query = teamBoardDbContext.UserInterviews
            .Where(IsNotIntern())
            .Where(IsTodayActive(today))
            .Where(HasActiveUser())
            .Where(HasResourceGroupAccess())
            .Where(HasDepartmentAccess(request.DepartmentId, canAccessAll))
            .Select(MapToDto())
            .OrderBy(x => x.UserInterviewFromUtc)
            .ThenBy(x => x.UserFullNameRu);

        return query.ToArrayAsync(cancellationToken);
    }

    private static Expression<Func<UserInterview, EmployeeInterviewDto>> MapToDto()
    {
        return x => new EmployeeInterviewDto
        {
            Id = x.Id,
            UserId = x.UserId,
            UserFullNameEng = x.User != null && (x.User.LastName != null || x.User.FirstName != null)
                ? (x.User.LastName + " " + x.User.FirstName).Trim()
                : null,
            UserFullNameRu = x.User != null && (x.User.LastNameRu != null || x.User.FirstNameRu != null)
                ? (x.User.LastNameRu + " " + x.User.FirstNameRu).Trim()
                : null,
            UserDepartmentId = x.User != null ? x.User.DepartmentId : null,
            UserResourceGroupId = x.User != null ? x.User.ResourceGroupId : null,
            Specialization = x.Specialization,
            UserInterviewDate = x.UserInterviewDate,
            UserInterviewFromUtc = x.UserInterviewFromUtc,
            Status = x.Status,
            UserPosition = x.User != null ? x.User.Position : null,
        };
    }

    private Expression<Func<UserInterview, bool>> HasDepartmentAccess(
        string? requestDepartmentId,
        bool canAccessAll)
    {
        return x =>
            (authenticatedUserContext.DepartmentId != null &&
                authenticatedUserContext.DepartmentId == x.User!.DepartmentId) ||
            (requestDepartmentId == null && canAccessAll) ||
            (requestDepartmentId == x.User!.DepartmentId && canAccessAll);
    }

    private static Expression<Func<UserInterview, bool>> IsTodayActive(DateOnly today)
    {
        return x => x.UserInterviewDate == today && x.IsActive;
    }

    private static Expression<Func<UserInterview, bool>> HasActiveUser()
    {
        return x => x.User != null && x.User.IsActive;
    }

    private static Expression<Func<UserInterview, bool>> IsNotIntern()
    {
        return x => x.User != null && x.User.Position != PortalRoleNames.Intern;
    }

    private Expression<Func<UserInterview, bool>> HasResourceGroupAccess()
    {
        return x =>
            authenticatedUserContext.ResourceGroupId == null ||
            authenticatedUserContext.ResourceGroupId == x.User!.ResourceGroupId;
    }

    public sealed record Query : IQuery<EmployeeInterviewDto[]>
    {
        /// <summary>
        /// If this property is empty, then user's departmentId will be used or all departments should be shown (for CEO/CTO)
        /// </summary>
        public string? DepartmentId { get; init; }
    }
}
