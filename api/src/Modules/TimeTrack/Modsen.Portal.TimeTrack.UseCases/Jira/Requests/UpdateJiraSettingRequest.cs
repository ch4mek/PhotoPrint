using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Validators;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class UpdateJiraSettingRequest : ICommandHandler<UpdateJiraSettingRequest.Command, JiraSettingDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IDataProtectionService _jiraSettingsProtectionService;
    private readonly IJiraService _jiraService;
    private readonly IGetJiraSettingDbQuery _getJiraSettingDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IJiraSettingValidator _jiraSettingValidator;

    public UpdateJiraSettingRequest(ITimeTrackDbContext timeTrackDbContext,
        IJiraSettingsDataProtectionFactory jiraSettingsDataProtectionFactory,
        IJiraService jiraService,
        IGetJiraSettingDbQuery getJiraSettingDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IJiraSettingValidator jiraSettingValidator)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _jiraSettingsProtectionService = jiraSettingsDataProtectionFactory.Create();
        _jiraService = jiraService;
        _getJiraSettingDbQuery = getJiraSettingDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _jiraSettingValidator = jiraSettingValidator;
    }

    public async Task<JiraSettingDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingEntity = await _getJiraSettingDbQuery
            .ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.JiraSettingNotFound);

        var shouldUpdatePat = !string.IsNullOrWhiteSpace(request.Pat);
        var pat = shouldUpdatePat
            ? request.Pat
            : _jiraSettingsProtectionService.Decrypt(existingEntity.Pat);

        if (string.IsNullOrWhiteSpace(pat))
        {
            throw new InvalidDataAppException();
        }

        var normalizedUrl = request.Url.TrimEnd('/').Trim().ToLower();
        await _jiraSettingValidator.ValidateAsync(
            request.Id,
            request.UserId,
            request.ProjectId,
            request.JiraProjectTag,
            normalizedUrl,
            cancellationToken);

        await _jiraService.CheckAccessAsync(request.Url, request.JiraProjectTag, pat, request.Email, cancellationToken);

        if (_authenticatedUserContext.GetRequiredUserId() != request.UserId)
        {
            throw new ActionNotAllowedException();
        }

        existingEntity.Url = normalizedUrl;
        existingEntity.JiraProjectTag = request.JiraProjectTag;
        existingEntity.UserId = request.UserId;
        existingEntity.ProjectId = request.ProjectId;
        existingEntity.Email = request.Email;

        if (shouldUpdatePat)
        {
            var encryptedPat = _jiraSettingsProtectionService.Encrypt(request.Pat);

            if (string.IsNullOrWhiteSpace(encryptedPat))
            {
                throw new InvalidDataAppException();
            }

            existingEntity.Pat = encryptedPat;
        }

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        return JiraSettingDto.Of(existingEntity);
    }

    internal sealed record Command(
        int Id,
        string Url,
        string? Pat,
        string JiraProjectTag,
        string? Email,
        int ProjectId,
        int UserId
    ) : ICommand<JiraSettingDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Url).NotEmpty().Must(JiraUrlValidator.IsValid);
            RuleFor(x => x.JiraProjectTag).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailType2Rule();
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
