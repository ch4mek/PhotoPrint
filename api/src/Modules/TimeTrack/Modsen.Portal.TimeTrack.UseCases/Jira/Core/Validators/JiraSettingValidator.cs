using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Core.Validators
{
    internal interface IJiraSettingValidator
    {
        Task ValidateAsync(int? jiraSettingId, int userId, int projectId, string jiraProjectTag, string normalizedUrl,
            CancellationToken cancellationToken);

        Task ValidateAsync(int userId, int projectId, string jiraProjectTag, string normalizedUrl,
            CancellationToken cancellationToken);
    }

    internal class JiraSettingValidator : IJiraSettingValidator
    {
        private readonly ITimeTrackDbContext _timeTrackDbContext;
        private readonly IClockService _clockService;

        public JiraSettingValidator(
            ITimeTrackDbContext timeTrackDbContext,
            IClockService clockService)
        {
            _timeTrackDbContext = timeTrackDbContext;
            _clockService = clockService;
        }

        public Task ValidateAsync(int userId, int projectId, string jiraProjectTag, string normalizedUrl,
            CancellationToken cancellationToken)
        {
            return ValidateAsync(null, userId, projectId, jiraProjectTag, normalizedUrl, cancellationToken);
        }

        public async Task ValidateAsync(
            int? jiraSettingId, int userId, int projectId, string jiraProjectTag, string normalizedUrl,
            CancellationToken cancellationToken)
        {
            var user = await _timeTrackDbContext.TimeTrackUsers
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

            var project = await _timeTrackDbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

            _ = await _timeTrackDbContext.ProjectUsers
                .Where(x => x.ProjectId == projectId)
                .Where(x => x.UserId == userId)
                .Where(x => x.UserOnProjectFrom <= _clockService.TodayUtc() &&
                            x.UserOnProjectTo == null || x.UserOnProjectTo >= _clockService.TodayUtc())
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidDataAppException("User not assigned to the project");

            var jiraSettingExists = await _timeTrackDbContext.JiraSettings.AnyAsync(
                jiraSetting => jiraSetting.IsActive &&
                    jiraSetting.ProjectId == project.Id &&
                    jiraSetting.UserId == user.UserId &&
                    jiraSetting.Url == normalizedUrl &&
                    jiraSetting.JiraProjectTag == jiraProjectTag &&
                    (jiraSettingId == null || jiraSetting.Id != jiraSettingId),
                cancellationToken);

            if (jiraSettingExists)
            {
                throw new EntityConflictException(PortalMessages.JiraSettingAlreadyExists);
            }
        }
    }
}
