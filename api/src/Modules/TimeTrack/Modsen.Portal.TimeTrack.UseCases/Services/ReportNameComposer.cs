using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Services;

internal class ReportNameComposer : IReportNameComposer
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IIdentityContract _identityContract;

    public ReportNameComposer(ITimeTrackDbContext timeTrackDbContext, IIdentityContract identityContract)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _identityContract = identityContract;
    }

    public Task<string> ComposeForSingleFileAsync(ProcessReportRequest.Command request, CancellationToken cancellationToken)
    {
        return ComposeForSingleFileAsync(
            request.UserId,
            request.ProjectId,
            request.ClientId,
            request.Year,
            request.MonthOfYear,
            request.Format,
            cancellationToken);
    }

    public async Task<string> ComposeForSingleFileAsync(
        int? userId,
        int? projectId,
        int? clientId,
        int? year,
        int[]? monthOfYear,
        DocumentType format,
        CancellationToken cancellationToken)
    {
        var startStringBuilder = new StringBuilder();

        if (userId is not null)
        {
            var userName = await GetUserNameAsync(userId.Value, cancellationToken);

            startStringBuilder.Append(userName);
            startStringBuilder.Append('_');
        }

        if (projectId is not null)
        {
            var projectName = await GetProjectNameAsync(projectId.Value, cancellationToken);

            startStringBuilder.Append(projectName);
            startStringBuilder.Append('_');
        }

        if (clientId is not null)
        {
            var clientName = await GetClientNameAsync(clientId.Value, cancellationToken);

            startStringBuilder.Append(clientName);
            startStringBuilder.Append('_');
        }        

        var endStringBuilder = new StringBuilder();
        endStringBuilder.Append("Report");
        endStringBuilder.Append(GetTimestampNamePart(year, monthOfYear));
        endStringBuilder.Append(format.GetExtensionWithDot());

        if (startStringBuilder.Length + endStringBuilder.Length > PortalSharedConstants.FileNameMaxLength)
        {
            var difference = startStringBuilder.Length + endStringBuilder.Length - PortalSharedConstants.FileNameMaxLength;

            startStringBuilder.Remove(startStringBuilder.Length - 1 - difference, difference);
        }

        return startStringBuilder.Append(endStringBuilder).ToString();        
    }

    public async Task<string> ComposeForPerUserArchiveAsync(ProcessReportRequest.Command request, CancellationToken cancellationToken)
    {
        var startStringBuilder = new StringBuilder();

        if (request.ProjectId is not null)
        {
            var projectName = await GetProjectNameAsync(request.ProjectId.Value, cancellationToken);

            startStringBuilder.Append(projectName);
            startStringBuilder.Append('_');
        }

        if (request.ClientId is not null)
        {
            var clientName = await GetClientNameAsync(request.ClientId.Value, cancellationToken);

            startStringBuilder.Append(clientName);
            startStringBuilder.Append('_');
        }

        if (request.DepartmentId is not null)
        {
            startStringBuilder.Append(await GetDepartmentNameAsync(request.DepartmentId, cancellationToken));
            startStringBuilder.Append('_');
        }

        if (request.ResourceGroupId is not null && request.ResourceGroupId != 0)
        {
            var groupName = await GetGroupNameAsync(request.ResourceGroupId.Value, cancellationToken);

            startStringBuilder.Append(groupName);
            startStringBuilder.Append('_');
        }

        var endStringBuilder = new StringBuilder();
        endStringBuilder.Append("Report");
        endStringBuilder.Append(GetTimestampNamePart(request.Year, request.MonthOfYear));
        endStringBuilder.Append(request.Format.GetExtensionWithDot());
        endStringBuilder.Append(".zip");

        if (startStringBuilder.Length + endStringBuilder.Length > PortalSharedConstants.FileNameMaxLength)
        {
            var difference = startStringBuilder.Length + endStringBuilder.Length - PortalSharedConstants.FileNameMaxLength;

            startStringBuilder.Remove(startStringBuilder.Length - 1 - difference, difference);
        }

        return startStringBuilder.Append(endStringBuilder).ToString();
    }

    private async Task<string> GetUserNameAsync(int userId, CancellationToken cancellationToken)
    {
        var userName = await _timeTrackDbContext.TimeTrackUsers
            .Where(x => x.UserId == userId)
            .Select(x => $"{x.FirstName} {x.LastName}")
            .FirstOrDefaultAsync(cancellationToken);

        if (userName is null)
        {
            throw new EntityNotFoundException($"User with id={userId} is not found");
        }

        return userName;
    }

    private async Task<string> GetProjectNameAsync(int projectId, CancellationToken cancellationToken)
    {
        var projectName = await _timeTrackDbContext.Projects
            .Where(x => x.Id == projectId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (projectName is null)
        {
            throw new EntityNotFoundException($"Project with id={projectId} is not found");
        }

        return projectName;
    }

    private async Task<string> GetClientNameAsync(int clientId, CancellationToken cancellationToken)
    {
        var clientName = await _timeTrackDbContext.Clients
            .Where(x => x.Id == clientId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (clientName is null)
        {
            throw new EntityNotFoundException($"Client with id={clientId} is not found");
        }

        return clientName;
    }

    private async Task<string> GetGroupNameAsync(int groupId, CancellationToken cancellationToken)
    {
        var resourceName = await _timeTrackDbContext.TimeTrackResourceGroups
            .Where(x => x.ResourceGroupId == groupId)
            .Select(x => x.ResourceGroupName)
            .FirstOrDefaultAsync(cancellationToken);

        if (resourceName is null)
        {
            throw new EntityNotFoundException($"Group with id={groupId} is not found");
        }

        return resourceName;
    }

    private static string GetTimestampNamePart(int? year, int[]? monthOfYear)
    {
        DateTime? filteredValuesFromDate = null;
        DateTime? filteredValuesToDate = null;

        if (year.HasValue)
        {
            if (monthOfYear is { Length: > 0 })
            {
                var firstMonth = monthOfYear.Min();
                var lastMonth = monthOfYear.Max();

                filteredValuesFromDate = new DateTime(year.Value, firstMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                filteredValuesToDate = new DateTime(year.Value, lastMonth, DateTime.DaysInMonth(year.Value, lastMonth), 0, 0, 0, DateTimeKind.Utc);
            }
            else
            {
                filteredValuesFromDate = new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                filteredValuesToDate = new DateTime(year.Value, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        var timestamp = "";

        if (filteredValuesFromDate != null && filteredValuesToDate != null)
        {
            timestamp += $"_[{
                filteredValuesFromDate.Value
                    .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
            }-{
                filteredValuesToDate.Value
                    .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
            }]";
        }

        return timestamp;
    }

    private async Task<string> GetDepartmentNameAsync(string departmentId, CancellationToken cancellationToken)
    {
        var departments = await _identityContract.GetDepartmentNames(cancellationToken);
        var department = departments.FirstOrDefault(d => d.DepartmentId == departmentId);

        if (department is null || department.FullName is null)
        {
            throw new InvalidDataAppException($"Department with id {departmentId} is not found");
        }

        return department.FullName!;
    }
}
