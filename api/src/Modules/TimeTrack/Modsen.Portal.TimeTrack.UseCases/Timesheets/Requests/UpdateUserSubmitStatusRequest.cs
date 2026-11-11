using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class UpdateUserSubmitStatusRequest
    : ICommandHandler<UpdateUserSubmitStatusRequest.Command, TimesheetSubmitStatusType>
{
    private readonly IUpdateSubmitStatusValidator _updateSubmitStatusValidator;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ISubmitStatusComposer _submitStatusComposer;
    private readonly ITimeTrackDbContext _dbContext;

    public UpdateUserSubmitStatusRequest(IUpdateSubmitStatusValidator updateSubmitStatusValidator,
        IAuthenticatedUserContext authenticatedUserContext,
        ISubmitStatusComposer submitStatusComposer,
        ITimeTrackDbContext dbContext)
    {
        _updateSubmitStatusValidator = updateSubmitStatusValidator;
        _authenticatedUserContext = authenticatedUserContext;
        _submitStatusComposer = submitStatusComposer;
        _dbContext = dbContext;
    }

    public async Task<TimesheetSubmitStatusType> Handle(Command request, CancellationToken cancellationToken)
    {
        await _updateSubmitStatusValidator.ValidateAsync(request.Model, cancellationToken);

        var trackedTime = await _dbContext.Timesheets
            .Where(timesheet =>
                timesheet.UserId == request.Model.UserId &&
                timesheet.ReportMonthOfYear == request.Model.Month &&
                timesheet.ReportYear == request.Model.Year)
            .GroupBy(_ => 1)
            .Select(grouping => new
            {
                Fact = grouping.Sum(timesheet => timesheet.FactTrackedInMinutes),
                Billable = grouping.Sum(timesheet => timesheet.BillableTrackedInMinutes)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var snapshot = await GetSnapshotAsync(request.Model, cancellationToken);
        var status = await UpdateStatusAsync(request.Model, trackedTime?.Fact ?? 0, trackedTime?.Billable ?? 0, snapshot,
            cancellationToken);

        return status.Status;
    }

    private async Task<string> GetSnapshotAsync(UpdateUserSubmitStatusDto dto,
        CancellationToken cancellationToken)
    {
        var timesheets = await _dbContext.Timesheets
            .Where(timesheet =>
                timesheet.UserId == dto.UserId &&
                timesheet.ReportYear == dto.Year &&
                timesheet.ReportMonthOfYear == dto.Month)
            .GroupBy(timesheet => timesheet.ProjectId)
            .Select(g => new ProjectTimesheetSnapshotCoreDto
            (
                g.Key,
                g.Sum(timesheet => timesheet.FactTrackedInMinutes),
                g.Sum(timesheet => timesheet.BillableTrackedInMinutes)
            ))
            .ToListAsync(cancellationToken);

        return JsonSerializer.Serialize(timesheets);
    }

    private async Task<TimesheetSubmitStatus> UpdateStatusAsync(UpdateUserSubmitStatusDto dto,
        int trackFact,
        int trackBillable,
        string snapshot,
        CancellationToken cancellationToken)
    {
        var existingStatus = await _dbContext.TimesheetSubmitStatuses
            .Where(submitStatus =>
                submitStatus.UserId == dto.UserId &&
                submitStatus.Month == dto.Month &&
                submitStatus.Year == dto.Year)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingStatus is null)
        {
            var submitStatus = new SubmitStatusCoreDto(
                UserId: dto.UserId,
                Year: dto.Year,
                Month: dto.Month,
                Status: dto.Status,
                FactTrackedInMinutesSum: trackFact,
                BillableTrackedInMinutesSum: trackBillable,
                Snapshot: snapshot);

            var createdStatus = await _submitStatusComposer.ComposeAsync(submitStatus, cancellationToken);
            return createdStatus;
        }

        existingStatus.UserId = dto.UserId;
        existingStatus.Year = dto.Year;
        existingStatus.Month = dto.Month;
        existingStatus.Date = new DateOnly(dto.Year, dto.Month, day: 1);
        existingStatus.Status = dto.Status;
        existingStatus.FactHoursSum = trackFact;
        existingStatus.BillableHoursSum = trackBillable;
        existingStatus.Snapshot = snapshot;
        existingStatus.DecisionBy = _authenticatedUserContext.GetRequiredUserId();

        _dbContext.TimesheetSubmitStatuses.Update(existingStatus);
        await _dbContext.SaveChanges(cancellationToken);

        return existingStatus;
    }

    internal sealed record Command(UpdateUserSubmitStatusDto Model)
        : ICommand<TimesheetSubmitStatusType>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.Model.UserId)
                .NotNull()
                .NotEmpty()
                .WithMessage(PortalMessages.UserIdHasNotBeenProvided);

            RuleFor(command => command.Model.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(PortalMessages.MonthMustBeBetweenOneAndTwelve);

            RuleFor(command => command.Model.Year)
                .GreaterThanOrEqualTo(2020)
                .WithMessage(PortalMessages.CannotUpdateStatusForYearEarlierThan2020);
        }
    }
}
