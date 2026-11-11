using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class UpdateTaskNameRequest
    : ICommandHandler<UpdateTaskNameRequest.Command, UpdateTaskNameResultDto>
{
    private readonly IUpdateTaskNameValidator _updateValidator;
    private readonly ITimeTrackDbContext _dbContext;

    private CancellationToken _cancellationToken;

    public UpdateTaskNameRequest(IUpdateTaskNameValidator updateValidator, ITimeTrackDbContext dbContext)
    {
        _updateValidator = updateValidator;
        _dbContext = dbContext;
    }

    public async Task<UpdateTaskNameResultDto> Handle(Command request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var timesheetsForUpdate = await GetTimesheets(request.RequestDto);

        await _updateValidator.ValidateAsync(request.RequestDto, timesheetsForUpdate, cancellationToken);

        await MergeTimeFromTimesheetsWithSameName(request.RequestDto, timesheetsForUpdate);

        var updatedCount = await UpdateTasksNamesAsync(timesheetsForUpdate, request.RequestDto);

        return new UpdateTaskNameResultDto(updatedCount, request.RequestDto.TaskName);
    }

    private async Task<List<TimesheetWithTrackedTimeCoreDto>> GetTimesheets(UpdateTaskNameDto updateDto)
    {
        var timesheets = await _dbContext.Timesheets
            .Where(timesheet => timesheet.UserId == updateDto.UserId)
            .Where(timesheet => timesheet.ProjectId == updateDto.ProjectId)
            .Where(timesheet => timesheet.TaskName == updateDto.OldTaskName)
            .Where(timesheet => updateDto.StartDate <= timesheet.ReportForDate &&
                updateDto.EndDate >= timesheet.ReportForDate)
            .Select(timesheet => new TimesheetWithTrackedTimeCoreDto
            {
                Id = timesheet.Id,
                ReportForDate = timesheet.ReportForDate,
                TrackedInMinutes = timesheet.FactTrackedInMinutes
            })
            .ToListAsync(_cancellationToken);

        if (timesheets.Count == 0)
        {
            throw new EntityNotFoundException();
        }

        return timesheets;
    }

    private async Task<int> UpdateTasksNamesAsync(List<TimesheetWithTrackedTimeCoreDto> timesheetsForUpdate,
        UpdateTaskNameDto updateDto)
    {
        var timesheetIdsForUpdating = timesheetsForUpdate.Select(timesheetDto => timesheetDto.Id);

        var timesheets = await _dbContext.Timesheets
            .Where(timesheet => timesheetIdsForUpdating.Contains(timesheet.Id))
            .ToListAsync(_cancellationToken);

        var updatedCount = 0;

        foreach (var timesheet in timesheets)
        {
            var matchingDto = timesheetsForUpdate
                .FirstOrDefault(dto => dto.ReportForDate == timesheet.ReportForDate);

            if (matchingDto != null)
            {
                timesheet.TaskName = updateDto.TaskName;

                timesheet.FactTrackedInMinutes = timesheetsForUpdate
                        .FirstOrDefault(dto => dto.ReportForDate == timesheet.ReportForDate)?.TrackedInMinutes ??
                    timesheet.FactTrackedInMinutes;
                updatedCount++;
            }
        }

        await _dbContext.SaveChanges(_cancellationToken);

        return updatedCount;
    }

    private async Task MergeTimeFromTimesheetsWithSameName(UpdateTaskNameDto updateDto,
        List<TimesheetWithTrackedTimeCoreDto> timesheetsToUpdate)
    {
        var timesheetsWithTheSameName = await _dbContext.Timesheets
            .Where(timesheet =>
                timesheet.UserId == updateDto.UserId &&
                timesheet.ProjectId == updateDto.ProjectId &&
                timesheetsToUpdate.Select(dto => dto.ReportForDate).Any(date => date == timesheet.ReportForDate) &&
                timesheet.TaskName == updateDto.TaskName)
            .Select(timesheet => new TimesheetWithTrackedTimeCoreDto
            {
                Id = timesheet.Id,
                TrackedInMinutes = timesheet.FactTrackedInMinutes,
                ReportForDate = timesheet.ReportForDate
            })
            .ToListAsync(_cancellationToken);

        foreach (var timesheetWithSameName in timesheetsWithTheSameName)
        {
            var timesheet = timesheetsToUpdate
                .FirstOrDefault(coreDto => coreDto.ReportForDate.Equals(timesheetWithSameName.ReportForDate));

            if (timesheet is not null)
            {
                timesheet.TrackedInMinutes += timesheetWithSameName.TrackedInMinutes;
            }
        }

        if (timesheetsWithTheSameName.Count != 0)
        {
            var timesheetIds = timesheetsWithTheSameName
                .Select(coreDto => coreDto.Id)
                .ToArray();

            await _dbContext.Timesheets
                .Where(timesheet => timesheetIds.Any(i => i == timesheet.Id))
                .ExecuteDeleteAsync(_cancellationToken);
        }
    }

    internal sealed record Command(UpdateTaskNameDto RequestDto) : ICommand<UpdateTaskNameResultDto>,
        ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.RequestDto).SetValidator(new UpdateTaskNameDto.Validator());
        }
    }
}
