using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Loaders;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetTimesheetsRequest
    : IQueryHandler<GetTimesheetsRequest.Query, PageableUserTimesheets>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimesheetValidator _getTimesheetValidator;
    private readonly IPageableUserTimesheetSnapshotsLoader _timesheetsLoader;

    public GetTimesheetsRequest(IAuthenticatedUserContext authenticatedUserContext,
        IGetTimesheetValidator getTimesheetValidator,
        IPageableUserTimesheetSnapshotsLoader timesheetsLoader)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getTimesheetValidator = getTimesheetValidator;
        _timesheetsLoader = timesheetsLoader;
    }

    public async Task<PageableUserTimesheets> Handle(Query request, CancellationToken cancellationToken)
    {
        await _getTimesheetValidator.ValidateAsync(request, cancellationToken);

        var timesheetsDto = CreateTimesheetsDto(request);

        var pageableTimesheets = await _timesheetsLoader.LoadAsync(timesheetsDto,
            cancellationToken);

        var items = pageableTimesheets.Items
            .Select(user => new UserTimesheetSnapshotDto
            {
                UserId = user.UserId,
                Color = user.Color,
                Role = user.Position,
                UserName = user.UserName,
                UserDepartmentId = user.UserDepartmentId,
                UserResourceGroupId = user.UserResourceGroupId,
                IsActive = user.IsActive,
                TotalFactTime = user.TotalFactTime,
                TotalBillableTime = user.TotalBillableTime,
                Status = user.Status,
                Timesheets = user.Timesheets.Select(t => new TimesheetSnapshotDto
                {
                    Id = t.Id,
                    ReportForDate = t.ReportForDate,
                    FactTrackedInMinutes = t.FactTrackedInMinutes,
                    BillableTrackedInMinutes = t.BillableTrackedInMinutes
                }).ToList()
            }).ToList();

        var pageableTimesheetDtos = items.ToPageable()
            .WithPaging(pageableTimesheets.Page, pageableTimesheets.PageSize)
            .WithTotal(pageableTimesheets.Total);

        return new PageableUserTimesheets(pageableTimesheetDtos);
    }

    private GetTimesheetsDto CreateTimesheetsDto(Query request)
    {
        var timesheetsDto = request.SearchFilters with
        {
            ResourceGroupId = request.SearchFilters.ResourceGroupId,
            ManagerId = request.SearchFilters.OnlyManagedProjects == true ? _authenticatedUserContext.UserId
                : null
        };

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && timesheetsDto.ManagerId is null)
        {
            timesheetsDto.DepartmentId = _authenticatedUserContext.DepartmentId;
        }
        else if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && timesheetsDto.ManagerId is not null)
        {
            timesheetsDto.DepartmentId = null;
        }
        else if(_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            timesheetsDto.ResourceGroupId = _authenticatedUserContext.ResourceGroupId;
        }

        return timesheetsDto;
    }

    internal sealed record Query(GetTimesheetsDto SearchFilters) : IQuery<PageableUserTimesheets>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.SearchFilters.ReportForDateFrom).NotNull().NotEmpty();
            RuleFor(query => query.SearchFilters.ReportForDateTo).NotNull().NotEmpty();
            RuleFor(query => query.SearchFilters.Top).NotNull();
            RuleFor(query => query.SearchFilters.Skip).NotNull();
            RuleFor(query => query.SearchFilters)
                .Must(options =>
                    options.Top > 0
                    && options.Skip > -1
                    && options.Skip % options.Top == 0)
                .WithMessage("Invalid pagination");
        }
    }
}
