using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetEmployeesEventsRequest : IQueryHandler<GetEmployeesEventsRequest.Query, ICollection<EmployeeEventDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;
    private readonly IPortalSettingsAccessor _portalSettingsAccessor;

    public GetEmployeesEventsRequest(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService,
        IPortalSettingsAccessor portalSettingsAccessor)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
        _portalSettingsAccessor = portalSettingsAccessor;
    }

    public async Task<ICollection<EmployeeEventDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var userRoles = _authenticatedUserContext.GetRoles();
        ValidateUserClaims(request, userRoles);

        var employeesEventsDaysSpan = await GetEmployeesEventsDaysSpan(cancellationToken);

        var dateBoundaries = GetDateBoundaries(employeesEventsDaysSpan);

        var users = await GetUsersModel(request, dateBoundaries, userRoles, cancellationToken);

        return ComposeEvents(users, dateBoundaries)
            .OrderBy(x => x.EventCelebrationDate)
            .ThenBy(x => x.FullNameRu)
            .ToArray();
    }

    private void ValidateUserClaims(Query request, IReadOnlyCollection<string> userRoles)
    {
        if (!_authenticatedUserContext.IsManager)
        {
            throw new ActionNotAllowedException("Accessing employees related events is allowed only for managers.");
        }

        if (request.DepartmentId.IsNullOrWhiteSpace() ||
            _authenticatedUserContext.DepartmentId == request.DepartmentId)
        {
            return;
        }

        if (!userRoles.IsInOneOfTheRoles(PortalRoleNames.Ceo, PortalRoleNames.Cto))
        {
            throw new ActionNotAllowedException("Accessing employees related events from another department is allowed only for CTO/CEO.");
        }
    }

    /// <summary>
    /// How long ahead should we show employees events
    /// </summary>
    private async Task<int> GetEmployeesEventsDaysSpan(CancellationToken cancellationToken)
    {
        var employeesEventsDaysSpanString = await _portalSettingsAccessor.GetStringValueByName(
            PortalSettingNames.EmployeesEventsDaysSpan,
            cancellationToken);

        if (int.TryParse(employeesEventsDaysSpanString, out var employeesEventsDaysSpan))
        {
            return employeesEventsDaysSpan;
        }

        throw new ArgumentException(message: "Invalid employees events days span value");
    }

    private (DateOnly From, DateOnly To) GetDateBoundaries(int employeesEventsDaysSpan)
    {
        var from = _clockService.TodayUtc();
        var to = _clockService.TodayUtc().AddDays(employeesEventsDaysSpan);

        return (from, to);
    }

    private Task<TempUserModel[]> GetUsersModel(
        Query request,
        (DateOnly From, DateOnly To) dateBoundaries,
        IReadOnlyCollection<string> userRoles,
        CancellationToken cancellationToken)
    {
        var canAccessAllEvents = userRoles.IsInOneOfTheRoles(
            PortalRoleNames.Ceo,
            PortalRoleNames.Cto);

        var query = _teamBoardDbContext.TeamBoardUsers
            .Where(IsNotIntern())
            .Where(IsActive())
            .Where(HasDepartmentAccess(request.DepartmentId, canAccessAllEvents))
            .Where(HasResourceGroupAccess())
            .Where(ComposeDateBoundariesComparingExpression(dateBoundaries))
            .Where(ComposeDateRestrictionsExpression(dateBoundaries))
            .Select(MapToTempUserModel());

        return query.ToArrayAsync(cancellationToken);
    }

    private static Expression<Func<TeamBoardUser, TempUserModel>> MapToTempUserModel()
    {
        return x => new TempUserModel
        {
            UserId = x.UserId,
            HireDate = x.HireDate,
            DateOfBirth = x.DateOfBirth,
            ResourceGroupId = x.ResourceGroupId,
            DepartmentId = x.DepartmentId,
            FullNameEng = !string.IsNullOrWhiteSpace(x.LastName) || !string.IsNullOrWhiteSpace(x.FirstName)
                ? (x.LastName + " " + x.FirstName).Trim()
                : null,
            FullNameRu = !string.IsNullOrWhiteSpace(x.LastNameRu) || !string.IsNullOrWhiteSpace(x.FirstNameRu)
                ? (x.LastNameRu + " " + x.FirstNameRu).Trim()
                : null,
            Position = x.Position,
        };
    }

    private Expression<Func<TeamBoardUser, bool>> HasDepartmentAccess(string? requestDepartmentId, bool canAccessAllEvents)
    {
        return x =>
            (_authenticatedUserContext.DepartmentId != null &&
                x.DepartmentId == _authenticatedUserContext.DepartmentId) ||
            (requestDepartmentId == null && canAccessAllEvents) ||
            (x.DepartmentId == requestDepartmentId && canAccessAllEvents);
    }

    private static Expression<Func<TeamBoardUser, bool>> IsActive()
    {
        return x => x.IsActive;
    }

    private static Expression<Func<TeamBoardUser, bool>> IsNotIntern()
    {
        return x => x.Position != PortalRoleNames.Intern;
    }

    private Expression<Func<TeamBoardUser, bool>> HasResourceGroupAccess()
    {
        return x => _authenticatedUserContext.ResourceGroupId == null ||
            x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId;
    }

    private static Expression<Func<TeamBoardUser, bool>> ComposeDateBoundariesComparingExpression((DateOnly From, DateOnly To) dateBoundaries)
    {
        // Within same year / month
        if (dateBoundaries.From.Month <= dateBoundaries.To.Month)
        {
            return x =>
                (
                    x.DateOfBirth != null &&
                    (
                        dateBoundaries.From.Month < x.DateOfBirth.Value.Month ||
                        (
                            x.DateOfBirth.Value.Month == dateBoundaries.From.Month &&
                            dateBoundaries.From.Day <= x.DateOfBirth.Value.Day
                        )
                    )
                    &&
                    (
                        x.DateOfBirth.Value.Month < dateBoundaries.To.Month ||
                        (
                            x.DateOfBirth.Value.Month == dateBoundaries.To.Month &&
                            x.DateOfBirth.Value.Day <= dateBoundaries.To.Day
                        )
                    )
                ) ||
                (
                    x.HireDate != null &&
                    (
                        dateBoundaries.From.Month < x.HireDate.Value.Month ||
                        (
                            x.HireDate.Value.Month == dateBoundaries.From.Month &&
                            dateBoundaries.From.Day <= x.HireDate.Value.Day
                        )
                    )
                    &&
                    (
                        x.HireDate.Value.Month < dateBoundaries.To.Month ||
                        (
                            x.HireDate.Value.Month == dateBoundaries.To.Month &&
                            x.HireDate.Value.Day <= dateBoundaries.To.Day
                        )
                    )
                );
        }

        // When one date from one year and the other, from another (30.12.2025 - 04.01.2026)
        return x =>
            (
                x.DateOfBirth != null &&
                (
                    (
                        x.DateOfBirth.Value.Month == dateBoundaries.From.Month &&
                        dateBoundaries.From.Day <= x.DateOfBirth.Value.Day
                    ) ||
                    (
                        x.DateOfBirth.Value.Month == dateBoundaries.To.Month &&
                        x.DateOfBirth.Value.Day <= dateBoundaries.To.Day
                    )
                )
            ) ||
            (
                x.HireDate != null &&
                (
                    (
                        x.HireDate.Value.Month == dateBoundaries.From.Month &&
                        dateBoundaries.From.Day <= x.HireDate.Value.Day
                    ) ||
                    (
                        x.HireDate.Value.Month == dateBoundaries.To.Month &&
                        x.HireDate.Value.Day <= dateBoundaries.To.Day
                    )
                )
            );
    }

    // This allows to show only anniversaries (e.g. users have been hired today and
    // even they fits in DateBoundaries, they shouldn't be displayed on the list)
    private static Expression<Func<TeamBoardUser, bool>> ComposeDateRestrictionsExpression((DateOnly From, DateOnly To) dateBoundaries)
    {
        return x =>
            (x.HireDate != null && x.HireDate.Value.Year < dateBoundaries.From.Year) ||
            (x.DateOfBirth != null && x.DateOfBirth.Value.Year < dateBoundaries.From.Year);
    }

    private static IEnumerable<EmployeeEventDto> ComposeEvents(TempUserModel[] users, (DateOnly From, DateOnly To) dateBoundaries)
    {
        foreach (var userModel in users)
        {
            if (userModel.DateOfBirth.HasValue)
            {
                foreach (var @event in ComposeEvents(userModel.DateOfBirth.Value, dateBoundaries, userModel,
                             EmployeeEventType.Birthday))
                {
                    yield return @event;
                }
            }

            if (userModel.HireDate.HasValue && userModel.HireDate.Value.Year < dateBoundaries.From.Year)
            {
                foreach (var @event in ComposeEvents(userModel.HireDate.Value, dateBoundaries, userModel,
                             EmployeeEventType.EmploymentAnniversary))
                {
                    yield return @event;
                }
            }
        }
    }

    private static IEnumerable<EmployeeEventDto> ComposeEvents(DateOnly date, (DateOnly From, DateOnly To) dateBoundaries, TempUserModel userModel, EmployeeEventType eventType)
    {
        var dateBoundaryForLocalDate = date.Month >= dateBoundaries.From.Month
            ? dateBoundaries.From
            : dateBoundaries.To;
        var localDate = date.GetLocalDateFromOldDate(dateBoundaryForLocalDate.Year);

        if (dateBoundaryForLocalDate.Month == localDate.Month || localDate.IsDateInRange(dateBoundaries.From, dateBoundaries.To))
        {
            yield return CreateEmployeeEvent(userModel,eventType, originalEventDate: date, localEventDate: localDate);
        }
    }

    private static EmployeeEventDto CreateEmployeeEvent(
        TempUserModel userModel,
        EmployeeEventType eventType,
        DateOnly originalEventDate,
        DateOnly localEventDate)
    {
        return new EmployeeEventDto(
            userId: userModel.UserId,
            fullNameEng: userModel.FullNameEng,
            fullNameRu: userModel.FullNameRu,
            eventCelebrationDate: localEventDate,
            eventType: eventType,
            departmentId: userModel.DepartmentId,
            resourceGroupId: userModel.ResourceGroupId,
            originalEventDate: originalEventDate,
            position: userModel.Position
        );
    }

    private sealed record TempUserModel
    {
        public int UserId { get; init; }
        public DateOnly? HireDate { get; init; }
        public DateOnly? DateOfBirth { get; init; }
        public string? FullNameEng { get; init; }
        public string? FullNameRu { get; init; }
        public string? DepartmentId { get; init; }
        public int? ResourceGroupId { get; init; }
        public string? Position { get; init; }
    }

    public sealed record Query : IQuery<ICollection<EmployeeEventDto>>
    {
        /// <summary>
        /// If this property is empty, then all departments should be shown
        /// </summary>
        public string? DepartmentId { get; init; }
    }
}
