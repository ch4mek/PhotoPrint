using System.Globalization;
using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Calculators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetTimesheetByUserIdRequest
    : IQueryHandler<GetTimesheetByUserIdRequest.Query, UserTimesheetsWithBlockedDaysDto>
{
    private readonly ITimesheetBlockedDaysCalculator _blockedDaysCalculator;
    private readonly ICanGetUserTimesheetBusinessRule _canGetUserTimesheet;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _dbContext;

    private CancellationToken _cancellationToken;

    public GetTimesheetByUserIdRequest(ITimesheetBlockedDaysCalculator blockedDaysCalculator,
        ICanGetUserTimesheetBusinessRule canGetUserTimesheet,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _blockedDaysCalculator = blockedDaysCalculator;
        _canGetUserTimesheet = canGetUserTimesheet;
        _dbContext = dbContext;
    }

    public async Task<UserTimesheetsWithBlockedDaysDto> Handle(Query request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var canGetUserTimesheet = await _canGetUserTimesheet.IsCorrespondsToBusiness(request.UserId,
            request.OnlyManagedProjects,
            cancellationToken);
        if (!canGetUserTimesheet)
        {
            throw new ActionNotAllowedException();
        }

        int? managerId = null;
        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            var isUserFromDepartment = await _dbContext.TimeTrackUsers.AnyAsync(user =>
                    user.UserId == request.UserId &&
                    user.DepartmentId == _authenticatedUserContext.DepartmentId!,
                cancellationToken);

            if (!isUserFromDepartment)
            {
                managerId = _authenticatedUserContext.UserId;
            }
        }

        var isAuthedUserHeadOrEmployee = _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsHeadOfDepartment();
        var onlyManagedProjects = request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects.Value;
        if (isAuthedUserHeadOrEmployee && onlyManagedProjects)
        {
            managerId = _authenticatedUserContext.UserId;
        }

        var pageableListOfTimesheets = await GetTimesheetsAsync(request.UserId, managerId, request.IsPersonalMode,
            request.QueryOptions);

        var blockedDays = await GetBlockedDaysAsync(request);

        return new UserTimesheetsWithBlockedDaysDto
        (
            PageableListOfItems : pageableListOfTimesheets,
            BlockedDays : blockedDays
        );
    }

    private async Task<PageableListOfItems<UserTimesheetDto>> GetTimesheetsAsync(int userId, int? managerId,
        bool isPersonalMode,
        ODataQueryOptions<TimesheetOdpDto> queryOptions)
    {
        var baseQuery = _dbContext.Timesheets
            .Include(timesheet => timesheet.Project)
            .Include(timesheet => timesheet.User)
            .Where(timesheet => timesheet.UserId == userId);

        if (managerId is not null)
        {
            baseQuery = baseQuery.Where(timesheet => timesheet.Project!.ManagerUserId == managerId);
        }

        var query = baseQuery.Select(TimesheetOdpDto.Map);
        var totalCount = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .Cast<TimesheetOdpDto>()
            .CountAsync(_cancellationToken);

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<UserTimesheetDto>.Value;
        }

        var timesheetsDtos = await queryOptions.ApplyTo(query)
            .Cast<TimesheetOdpDto>()
            .Select(UserTimesheetDto.Map)
            .ToListAsync(_cancellationToken);

        if (!_authenticatedUserContext.IsManager)
        {
            var canUserGetBillableHours =
                _authenticatedUserContext.HasClaim(IdentityClaimTypeNames.AvailableProjectManagerRole) &&
                !isPersonalMode;

            timesheetsDtos = timesheetsDtos
                .Where(timesheet =>
                    _authenticatedUserContext.HasClaimWithValue(
                        PortalClaimTypeNames.TrackBillableWorkingHoursOnProject,
                        timesheet.ProjectId.ToString(CultureInfo.InvariantCulture)) ||
                    canUserGetBillableHours ||
                    timesheet.FactTrackedInMinutes != 0)
                .ToList();

            foreach (var userTimesheet in timesheetsDtos)
            {
                var canTrackAndGetBillableTime = _authenticatedUserContext
                        .HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject,
                            userTimesheet.ProjectId.ToString(CultureInfo.InvariantCulture)) ||
                    canUserGetBillableHours;

                if (!canTrackAndGetBillableTime)
                {
                    userTimesheet.BillableTrackedInMinutes = null;
                }
            }
        }

        return timesheetsDtos.ToPageable(queryOptions, totalCount);
    }

    private async Task<List<DateOnly>> GetBlockedDaysAsync(Query request)
    {
        var userRole = await _dbContext.TimeTrackUsers
            .Where(user => user.UserId == request.UserId)
            .Select(user => new { user.Position })
            .FirstOrDefaultAsync(_cancellationToken);

        var isEmployeeOrSysAdmin = userRole!.Position!.IsEmployee() || userRole.Position!.IsSystemAdministrator();

        var (startDate, endDate) = QueryStringExtensions.ExtractDatesFromQueryString(request.QueryOptions.Filter?.RawValue ?? string.Empty);

        var blockedDays = new List<DateOnly>();
        if (isEmployeeOrSysAdmin && startDate.HasValue && endDate.HasValue)
        {
            blockedDays = await _blockedDaysCalculator.CalculateAsync(request.UserId, startDate.Value, endDate.Value,
                _cancellationToken);
        }

        return blockedDays;
    }

    internal sealed record Query(ODataQueryOptions<TimesheetOdpDto> QueryOptions,
        int UserId,
        bool? OnlyManagedProjects,
        bool IsPersonalMode) : IQuery<UserTimesheetsWithBlockedDaysDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();

            RuleFor(query => query.UserId)
                .NotEmpty()
                .NotNull()
                .WithMessage(PortalMessages.UserIdHasNotBeenProvided);
        }
    }
}
