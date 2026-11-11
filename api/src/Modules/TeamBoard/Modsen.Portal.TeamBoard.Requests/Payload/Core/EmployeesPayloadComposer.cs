using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.Inquiries.Contract.Models;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Loaders;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.ContractDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core;

internal interface IEmployeesPayloadComposer
{
    Task<PayloadCoreDto> Compose(int year, int[] userIds, int totalUsers, PayloadFilterCoreDto filter, CancellationToken token);
}

internal class EmployeesPayloadComposer : IEmployeesPayloadComposer
{
    private readonly IPayloadUserDetailsLoader _payloadUserDetailsLoader;
    private readonly IPayloadCalendarWorkingHourDetailsLoader _calendarWorkingHourDetailsLoader;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly IWorkingDaysCalculator _workingDaysCalculator;
    private readonly IUserProjectHoursCalculator _userProjectHoursCalculator;
    private readonly IIsUserAssignedBusinessRule _isUserAssignedBusinessRule;
    private readonly IWorkingHoursInMonthCalculator _workingHoursInMonthCalculator;
    private readonly IUserHoursCalculator _userHoursCalculator;
    private readonly ITotalUserTimeByMonthsCalculator _totalUserTimeByMonthsCalculator;
    private readonly IUserActivityMonthsCalculator _userActivityMonthsCalculator;
    private readonly IClockService _clockService;
    private readonly IIdentityContract _identityContract;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IInquiriesContract _inquiriesContract;
    private readonly decimal _minutesInHour = 60m;
    private readonly int _defaultCountryId;

    public EmployeesPayloadComposer(
        IPayloadUserDetailsLoader payloadUserDetailsLoader,
        IPayloadCalendarWorkingHourDetailsLoader calendarWorkingHourDetailsLoader,
        ITimeTrackContract timeTrackContract,
        IConfiguration configuration,
        IWorkingDaysCalculator workingDaysCalculator,
        IUserProjectHoursCalculator userProjectHoursCalculator,
        IIsUserAssignedBusinessRule isUserAssignedBusinessRule,
        IWorkingHoursInMonthCalculator workingHoursInMonthCalculator,
        IUserHoursCalculator userHoursCalculator,
        ITotalUserTimeByMonthsCalculator totalUserTimeByMonthsCalculator,
        IUserActivityMonthsCalculator userActivityMonthsCalculator,
        IClockService clockService,
        IIdentityContract identityContract,
        IAuthenticatedUserContext authenticatedUserContext,
        IInquiriesContract inquiriesContract)
    {
        _payloadUserDetailsLoader = payloadUserDetailsLoader;
        _calendarWorkingHourDetailsLoader = calendarWorkingHourDetailsLoader;
        _timeTrackContract = timeTrackContract;
        _workingDaysCalculator = workingDaysCalculator;
        _defaultCountryId = configuration.GetValue<int>("AppSettings:TeamboardSettings:DefaultCountryId");
        _userProjectHoursCalculator = userProjectHoursCalculator;
        _isUserAssignedBusinessRule = isUserAssignedBusinessRule;
        _workingHoursInMonthCalculator = workingHoursInMonthCalculator;
        _userHoursCalculator = userHoursCalculator;
        _totalUserTimeByMonthsCalculator = totalUserTimeByMonthsCalculator;
        _userActivityMonthsCalculator = userActivityMonthsCalculator;
        _clockService = clockService;
        _identityContract = identityContract;
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesContract = inquiriesContract;
    }

    public async Task<PayloadCoreDto> Compose(int year, int[] userIds, int totalUsers, PayloadFilterCoreDto filter, CancellationToken token)
    {
        var filterDepartmentIds = GetFilterDepartmentIds(filter.DepartmentIds);

        var payload = await _timeTrackContract.GetTrackedTimePayloadForUsers(userIds, year, filter.ProjectName, filterDepartmentIds, token);

        var users = await _payloadUserDetailsLoader.Load(userIds, year, filter.ProjectName, token);

        var ownExpenseAbsence = await _inquiriesContract.GetUsersInquiriesAsync(userIds, token, InquiryType.OwnExpense, UserInquiryStatus.Approved);

        var calendarExclusion = await _inquiriesContract.GetCalendarExclusionAsync(year, token);

        var calendarDetails = await _calendarWorkingHourDetailsLoader.Load(year, token);

        var dateRanges = await _identityContract.GetUsersActivePeriodsInDepartments(userIds, filterDepartmentIds, token);

        CheckCalendar(calendarDetails);

        var workingDaysInMonth = _workingDaysCalculator.GetWorkingDaysInMonth(year);

        var payloadDto = new PayloadCoreDto
        {
            Year = year,
            UsersTotal = totalUsers,
            Projects = payload.Projects
                .Select(project => new PayloadProjectCoreDto
                {
                    ProjectId = project.ProjectId,
                    ClientId = project.ClientId,
                    Name = project.Name
                })
                .ToArray(),
            Clients = payload.Clients
                .Select(client => new PayloadClientCoreDto
                {
                    ClientId = client.ClientId,
                    Name = client.Name,
                    Color = client.Color,
                })
                .ToArray(),
            Users = users
                .GroupJoin(
                    inner: payload.Users,
                    innerKeySelector: x => x.UserId,
                    outerKeySelector: x => x.UserId,
                    resultSelector: (userDetailsDto, payloadUser) => new
                    {
                        UserDetails = userDetailsDto,
                        PayloadUserCanBeNull = payloadUser.FirstOrDefault()
                    })
                .Select(leftJoinResult =>
                {
                    return new PayloadUserCoreDto
                    {
                        UserId = leftJoinResult.UserDetails.UserId,
                        IsActive = leftJoinResult.UserDetails.IsActive,
                        DepartmentId = leftJoinResult.UserDetails.DepartmentId,
                        ResourceGroupId = leftJoinResult.UserDetails.ResourceGroupId,
                        ResourceGroupName = leftJoinResult.UserDetails.ResourceGroupName,
                        FirstName = leftJoinResult.UserDetails.FirstName,
                        LastName = leftJoinResult.UserDetails.LastName,
                        FirstNameRu = leftJoinResult.UserDetails.FirstNameRu,
                        LastNameRu = leftJoinResult.UserDetails.LastNameRu,
                        Color = leftJoinResult.UserDetails.Color,
                        SpecializationName = leftJoinResult.UserDetails.PrimarySpecializationName,
                        SpecializationSeniority = leftJoinResult.UserDetails.SpecializationSeniority,
                        Comments = leftJoinResult.UserDetails.Comment,
                        Plan = leftJoinResult.UserDetails.Plan,
                        NeedProject = leftJoinResult.UserDetails.NeedProject,
                        IsHeadOfDepartment = leftJoinResult.UserDetails.IsHeadOfDepartment,
                        HasContracts = leftJoinResult.UserDetails.IsUserHasContracts,
                        HireDate = leftJoinResult.UserDetails.HireDate,
                        Projects = ResolveProjectDetails(leftJoinResult.UserDetails.ProjectDetails),
                        HourDetails = GetCalendarDetailsWithEmptyFields(calendarDetails)
                                .GroupJoin(
                                    inner: GetHourDetailsWithEmptyFields(leftJoinResult.PayloadUserCanBeNull?.HourDetails ?? []),
                                    innerKeySelector: payloadUserContractDto => payloadUserContractDto.MonthOfYear,
                                    outerKeySelector: calendarWorkingHourDetails => calendarWorkingHourDetails.MonthOfYear,
                                    resultSelector: (calendarWorkingHourDetails, userMonthHours) => new
                                    {
                                        Year = year,
                                        HourDetailsByMonth = userMonthHours.First(),
                                        WorkingHoursPerMonth = _workingHoursInMonthCalculator.CalculateWorkingHoursInMonth(new(
                                            leftJoinResult.UserDetails.RegisteredLocations,
                                            calendarWorkingHourDetails.CountryHours,
                                            _defaultCountryId,
                                            year,
                                            calendarWorkingHourDetails.MonthOfYear)),
                                        OwnExpenseAbsenceHours = GetUserOwnExpenseAbsenceHoursByMonth(
                                            leftJoinResult.UserDetails.UserId,
                                            year,
                                            calendarWorkingHourDetails.MonthOfYear,
                                            leftJoinResult.UserDetails.RegisteredLocations,
                                            ownExpenseAbsence,
                                            calendarExclusion),
                                        CurrentMonth = calendarWorkingHourDetails.MonthOfYear
                                    })
                                .Select(details =>
                                {
                                    var plannedFte = leftJoinResult.UserDetails.UserMonthPayloadDetails
                                        .Where(x => x.PayloadMonthOfYear == details.CurrentMonth)
                                        .Select(x => x.EstimatedFte)
                                        .FirstOrDefault(defaultValue: 0m);

                                    var workingHoursPerMonth = details.WorkingHoursPerMonth > details.OwnExpenseAbsenceHours
                                        ? details.WorkingHoursPerMonth - details.OwnExpenseAbsenceHours
                                        : 0;

                                    var payloadProjectDetails = GetProjectsWithEmptyTrackFields(
                                        leftJoinResult.UserDetails.ProjectDetails,
                                        details.HourDetailsByMonth,
                                        payload.Projects.Select(project => project.ProjectId).ToHashSet(),
                                        details.CurrentMonth,
                                        year)
                                            .Join(
                                                inner: leftJoinResult.UserDetails.ProjectDetails,
                                                innerKeySelector: x => x.ProjectId,
                                                outerKeySelector: x => x.ProjectId,
                                                resultSelector: (projectHours, projectDetails) => new { projectHours, projectDetails })
                                            .Select(joinResult =>
                                            {
                                                var isUserAssigned = _isUserAssignedBusinessRule.IsUserAssigned(
                                                    joinResult.projectDetails.UserOnProjectFrom,
                                                    joinResult.projectDetails.UserOnProjectTo,
                                                    details.CurrentMonth,
                                                    year);

                                                var trackedBillable = Round(joinResult.projectHours.TotalBillableTrackedInMinutes / _minutesInHour);

                                                var projectHours = _userProjectHoursCalculator.CalculateUserProjectHoursForSingleAssign(new(
                                                    joinResult.projectDetails.ProjectId,
                                                    year,
                                                    details.CurrentMonth,
                                                    workingDaysInMonth[details.CurrentMonth],
                                                    workingHoursPerMonth,
                                                    joinResult.projectDetails.UserOnProjectFrom,
                                                    joinResult.projectDetails.UserOnProjectTo,
                                                    joinResult.projectDetails.Fte,
                                                    trackedBillable,
                                                    dateRanges?[leftJoinResult.UserDetails.UserId]));

                                                var isDuplicate = joinResult.projectDetails.UserOnProjectFrom != joinResult.projectHours.UserOnProjectFrom &&
                                                    joinResult.projectDetails.UserOnProjectTo != joinResult.projectHours.UserOnProjectTo &&
                                                    trackedBillable != 0 &&
                                                    projectHours.RequiredHours != 0;

                                                if (!isUserAssigned || isDuplicate)
                                                {
                                                    return (joinResult.projectDetails.UserOnProjectFrom, joinResult.projectDetails.UserOnProjectTo, new()
                                                    {
                                                        ProjectId = -1,
                                                    });
                                                }

                                                projectHours.RequiredHours = joinResult.projectDetails.IsTimeOnProjectBillable
                                                    ? projectHours.RequiredHours : 0;

                                                return (joinResult.projectDetails.UserOnProjectFrom,
                                                    joinResult.projectDetails.UserOnProjectTo,
                                                    projectHours);
                                            })
                                            .GroupBy(payloadUserProjectHoursCoreDto =>
                                                payloadUserProjectHoursCoreDto.projectHours.ProjectId)
                                            .Where(group => group.Key != -1)
                                            .Select(group => _userProjectHoursCalculator.CalculateUserProjectHoursForGroupAssigns(group, workingHoursPerMonth))
                                            .ToArray();

                                    var totalUserTimeByMonth = _totalUserTimeByMonthsCalculator.Calculate(
                                        new(
                                            leftJoinResult.UserDetails.ContractDates,
                                            leftJoinResult.UserDetails.IsUserHasContracts,
                                            leftJoinResult.UserDetails.HireDate,
                                            year,
                                            details.CurrentMonth,
                                            workingHoursPerMonth,
                                            leftJoinResult.UserDetails.CareerStartedAt,
                                            dateRanges?[leftJoinResult.UserDetails.UserId]));

                                    var userActivityMonths = _userActivityMonthsCalculator.Calculate(
                                        leftJoinResult.UserDetails.ContractDates,
                                        leftJoinResult.UserDetails.HireDate,
                                        leftJoinResult.UserDetails.CareerStartedAt,
                                        year,
                                        dateRanges?[leftJoinResult.UserDetails.UserId]);

                                    return _userHoursCalculator.CalculateUserHours(new UserHourCalculatorDto
                                        (payloadProjectDetails,
                                        details.CurrentMonth,
                                        year,
                                        userActivityMonths,
                                        plannedFte,
                                        workingHoursPerMonth,
                                        totalUserTimeByMonth,
                                        dateRanges?[leftJoinResult.UserDetails.UserId]));
                                })
                                .ToArray()
                    };
                })
                .OrderBy(x => Array.IndexOf(userIds, x.UserId))
                .ToArray()
        };

        return payloadDto;
    }

    private IReadOnlyCollection<string>? GetFilterDepartmentIds(IReadOnlyCollection<string>? departmentIds)
    {
        var filterDepartmentIds = departmentIds;

        if ((_authenticatedUserContext.IsHeadOfDepartment() || _authenticatedUserContext.IsGroupManager()) &&
            _authenticatedUserContext.DepartmentId != null)
        {
            filterDepartmentIds = departmentIds != null
                ? departmentIds.Intersect([_authenticatedUserContext.DepartmentId]).ToList()
                : (IReadOnlyCollection<string>)[_authenticatedUserContext.DepartmentId];
        }

        return filterDepartmentIds;
    }

    private static List<PayloadUserMonthHoursContractDto> GetHourDetailsWithEmptyFields(
        IReadOnlyCollection<PayloadUserMonthHoursContractDto> payloadUserMonthHoursContracts)
    {
        var hourDetailsWithEmptyFields = new List<PayloadUserMonthHoursContractDto>();

        for (var i = 1; i <= 12; i++)
        {
            var existMonth = payloadUserMonthHoursContracts
                .FirstOrDefault(payloadUserMonthHoursContract => payloadUserMonthHoursContract.MonthOfYear == i);

            if (existMonth is not null)
            {
                hourDetailsWithEmptyFields.Add(existMonth);
            }
            else
            {
                hourDetailsWithEmptyFields.Add(new PayloadUserMonthHoursContractDto
                {
                    TotalBillableTrackedInMinutes = 0,
                    TotalFactTrackedInMinutes = 0,
                    MonthOfYear = i,
                    Projects = []
                });
            }
        }

        return hourDetailsWithEmptyFields;
    }

    private int GetUserOwnExpenseAbsenceHoursByMonth(
        int userId,
        int year,
        int month,
        IReadOnlyCollection<RegisteredLocationHistoryCoreDto> registeredLocations,
        List<UserInquiryDto> inquiries,
        List<CalendarExclusionDto> calendarExclusion)
    {
        var firstDay = new DateOnly(year, month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var registeredLocationId = registeredLocations
            .Where(x => x.DateFrom <= lastDay)
            .Where(x => x.DateTo == null || x.DateTo >= lastDay)
            .Select(x => x.RegisteredLocationId)
            .FirstOrDefault();

        if (registeredLocationId == 0)
        {
            registeredLocationId = _defaultCountryId;
        }

        var calendarExclusionByUserCountry = calendarExclusion.Where(x => x.CountryId == registeredLocationId).ToList();

        var daysCount = inquiries
            .Where(x => x.UserId == userId)
            .Where(x => x.DateFrom <= lastDay && x.DateTo >= firstDay)
            .Sum(x =>
            {
                var start = x.DateFrom > firstDay ? x.DateFrom : firstDay;
                var end = x.DateTo < lastDay ? x.DateTo : lastDay;

                var current = start;
                var days = 0;

                while (current <= end)
                {
                    if (IsWorkingDay(current, calendarExclusionByUserCountry))
                    {
                        days++;
                    }

                    current = current.AddDays(1);
                }

                return days;
            });

        return daysCount * 8;
    }

    private bool IsWorkingDay(DateOnly date, List<CalendarExclusionDto> calendarExclusion)
    {
        var workingDay = _workingDaysCalculator.IsWorkingDay(date);

        if (calendarExclusion.Count == 0)
        {
            return workingDay;
        }

        if (calendarExclusion.Any(x => x.Date == date))
        {
            workingDay = false;
        }

        if (calendarExclusion.Any(x => x.NewDate == date))
        {
            workingDay = true;
        }

        return workingDay;
    }

    private List<PayloadProjectHoursContractDto> GetProjectsWithEmptyTrackFields(
        IReadOnlyCollection<UserProjectDetailsCoreDto> userProjectDetails,
        PayloadUserMonthHoursContractDto payloadUserMonthHoursContract,
        HashSet<int> existProjects,
        int monthOfYear,
        int year)
    {
        var projectsWithEmptyTrackFields = new List<PayloadProjectHoursContractDto>();

        foreach (var projectDetail in userProjectDetails)
        {
            var query = payloadUserMonthHoursContract.Projects
                .Where(project => project.ProjectId == projectDetail.ProjectId)
                .ToArray();

            if (query.Length > 1)
            {
                query = query
                    .Where(project => project.UserOnProjectFrom == projectDetail.UserOnProjectFrom)
                    .Where(project => project.UserOnProjectTo == projectDetail.UserOnProjectTo)
                    .ToArray();
            }

            var project = query.FirstOrDefault();

            var isUserAssigned = _isUserAssignedBusinessRule.IsUserAssigned(
                projectDetail.UserOnProjectFrom,
                projectDetail.UserOnProjectTo,
                monthOfYear,
                year);

            if (project is not null)
            {
                projectsWithEmptyTrackFields.Add(project);
            }
            else if (isUserAssigned && existProjects.Contains(projectDetail.ProjectId))
            {
                projectsWithEmptyTrackFields.Add(new PayloadProjectHoursContractDto
                {
                    ProjectId = projectDetail.ProjectId,
                    TotalBillableTrackedInMinutes = 0,
                    TotalFactTrackedInMinutes = 0,
                });
            }
        }

        return projectsWithEmptyTrackFields;
    }

    private static List<CalendarWorkingHourDetailsCoreDto> GetCalendarDetailsWithEmptyFields(
        CalendarDto calendarDto)
    {
        var calendarDetailsWithEmptyFields = new List<CalendarWorkingHourDetailsCoreDto>();

        for (var i = 1; i <= 12; i++)
        {
            var existMonth = calendarDto.CalendarWorkingHourDetails
                .FirstOrDefault(detail => detail.MonthOfYear == i);

            if (existMonth is null)
            {
                existMonth = new CalendarWorkingHourDetailsCoreDto
                {
                    CountryHours = [],
                    MonthOfYear = i,
                };
            }

            var countryHours = new List<CalendarCountryHourDetailsCoreDto>();

            foreach (var countryId in calendarDto.Countries.Select(x => x.Id))
            {
                var existCountry = existMonth.CountryHours
                    .FirstOrDefault(existCountry => existCountry.CountryId == countryId);

                if (existCountry is not null)
                {
                    countryHours.Add(existCountry);
                }
                else
                {
                    countryHours.Add(new CalendarCountryHourDetailsCoreDto
                    {
                        Hours = -1,
                        CountryId = countryId
                    });
                }
            }

            existMonth.CountryHours = [.. countryHours];
            calendarDetailsWithEmptyFields.Add(existMonth);
        }

        return calendarDetailsWithEmptyFields;
    }

    private void CheckCalendar(CalendarDto calendar)
    {
        for (var i = 1; i <= 12; i++)
        {
            var workingHours = calendar.CalendarWorkingHourDetails.FirstOrDefault(workingHour => workingHour.MonthOfYear == i);

            if (workingHours is null)
            {
                throw new EntityNotFoundException("WH are not set in calendar for Belarus for chosen year");
            }

            var countryHour = workingHours.CountryHours.FirstOrDefault(countryHour => countryHour.CountryId == _defaultCountryId);

            if (countryHour is null || countryHour.Hours <= 0)
            {
                throw new EntityNotFoundException("WH are not set in calendar for Belarus for chosen year");
            }
        }
    }

    private static decimal Round(decimal val)
    {
        return Math.Round(val, 2);
    }

    private PayloadAssignedProject[] ResolveProjectDetails(IReadOnlyCollection<UserProjectDetailsCoreDto> projectDetails)
    {
        var today = _clockService.TodayUtc();

        return projectDetails.Select(x => new PayloadAssignedProject
        {
            ProjectId = x.ProjectId,
            IsActiveAssignement = x.UserOnProjectFrom <= today &&
                (!x.UserOnProjectTo.HasValue || today <= x.UserOnProjectTo),
            IsFutureActiveAssignment = today <= x.UserOnProjectFrom,
            IsTimeOnProjectBillable = x.IsTimeOnProjectBillable
        }).OrderBy(x => x.ProjectId)
        .ThenByDescending(x => x.IsActiveAssignement)
        .ThenByDescending(x => x.IsFutureActiveAssignment)
        .DistinctBy(x => x.ProjectId)
        .ToArray();
    }
}
