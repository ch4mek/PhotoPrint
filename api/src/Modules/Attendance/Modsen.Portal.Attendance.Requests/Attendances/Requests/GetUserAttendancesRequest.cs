using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Attendance.Requests.Attendances.Requests;

internal sealed class GetUserAttendancesRequest
    : IQueryHandler<GetUserAttendancesRequest.Query, PageableListOfItems<UserAttendanceDto>>
{
    private readonly IAttendanceDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IAttendanceTotalCalculator _attendanceTotalCalculator;

    public GetUserAttendancesRequest(
        IAttendanceDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IAttendanceTotalCalculator attendanceTotalCalculator)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _attendanceTotalCalculator = attendanceTotalCalculator;
    }

    public async Task<PageableListOfItems<UserAttendanceDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        await EnsureUserCanAccessAsync(request.UserId, cancellationToken);

        var entities = await _dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.UserId == request.UserId)
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);

        var rows = entities.Select(ToODataRow).AsQueryable();

        var total = request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(rows)
            .Cast<OdpUserAttendanceRow>()
            .Count();

        if (total == 0)
        {
            return EmptyPageableListOfItems<UserAttendanceDto>.Value;
        }

        var filteredRows = request.QueryOptions
            .ApplyTo(rows)
            .Cast<OdpUserAttendanceRow>()
            .ToList();

        var byId = entities.ToDictionary(e => e.Id);
        var items = filteredRows.Select(r => MapToDto(byId[r.Id]));

        return items.ToPageable(request.QueryOptions, total);
    }

    private OdpUserAttendanceRow ToODataRow(AttendanceEntity e)
    {
        var (date, timeOnly) =
            _attendanceTotalCalculator.GetLocalDateAndTimeFromCheckedAtUtc(e.CheckedAtUtc, e.Location);

        return new OdpUserAttendanceRow
        {
            Id = e.Id,
            Date = date,
            Time = timeOnly.Hour * 60 + timeOnly.Minute,
            Direction = e.Direction,
            Door = e.Door,
            Location = e.Location.ToString(),
        };
    }

    private UserAttendanceDto MapToDto(AttendanceEntity a)
    {
        var (date, time) =
            _attendanceTotalCalculator.GetLocalDateAndTimeFromCheckedAtUtc(a.CheckedAtUtc, a.Location);

        return new UserAttendanceDto
        {
            Date = date,
            Time = time,
            Direction = a.Direction,
            Door = a.Door,
            Location = a.Location.ToString(),
        };
    }

    private async Task EnsureUserCanAccessAsync(int requestedUserId, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.UserId == requestedUserId)
        {
            return;
        }

        var roles = _authenticatedUserContext.GetRoles();
        if (roles.IsCeo() || roles.IsCto())
        {
            return;
        }

        var requestedUser = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.UserId == requestedUserId)
            .Select(x => new
            {
                x.DepartmentId,
                x.ResourceGroupId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (requestedUser is null)
        {
            return;
        }

        if (roles.IsGroupManager())
        {
            if (requestedUser.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
            {
                return;
            }

            throw new ActionNotAllowedException(PortalMessages.GroupManagerDoNotHaveAccessToOtherGroup);
        }

        if (roles.IsHeadOfDepartment())
        {
            if (requestedUser.DepartmentId == _authenticatedUserContext.DepartmentId)
            {
                return;
            }

            throw new ActionNotAllowedException(PortalMessages.HeadDoNotHaveAccessToOtherDepartments);
        }

        throw new ActionNotAllowedException(PortalMessages.CannotAccessOtherUserData);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<OdpUserAttendanceRow> QueryOptions)
        : IQuery<PageableListOfItems<UserAttendanceDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
