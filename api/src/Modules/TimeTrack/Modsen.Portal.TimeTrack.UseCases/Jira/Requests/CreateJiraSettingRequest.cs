using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Validators;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class CreateJiraSettingRequest : ICommandHandler<CreateJiraSettingRequest.Command, JiraSettingDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IDataProtectionService _jiraSettingsProtectionService;
    private readonly IJiraService _jiraService;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IJiraSettingValidator _jiraSettingValidator;

    public CreateJiraSettingRequest(ITimeTrackDbContext timeTrackDbContext,
        IJiraSettingsDataProtectionFactory jiraSettingsDataProtectionFactory,
        IJiraService jiraService,
        IAuthenticatedUserContext authenticatedUserContext,
        IJiraSettingValidator jiraSettingValidator)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _jiraSettingsProtectionService = jiraSettingsDataProtectionFactory.Create();
        _jiraService = jiraService;
        _authenticatedUserContext = authenticatedUserContext;
        _jiraSettingValidator = jiraSettingValidator;
    }


    public async Task<JiraSettingDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (request.UserId != _authenticatedUserContext.GetRequiredUserId())
        {
            throw new ActionNotAllowedException();
        }

        var normalizedUrl = request.Url.TrimEnd('/').Trim().ToLower();
        await _jiraSettingValidator.ValidateAsync(
            request.UserId,
            request.ProjectId,
            request.JiraProjectTag,
            normalizedUrl,
            cancellationToken);

        await _jiraService.CheckAccessAsync(normalizedUrl, request.JiraProjectTag, request.Pat, request.Email, cancellationToken);

        var encryptedPat = _jiraSettingsProtectionService.Encrypt(request.Pat);

        if (string.IsNullOrWhiteSpace(encryptedPat))
        {
            throw new InvalidDataAppException();
        }

        var jiraSetting = new JiraSetting
        {
            Url = normalizedUrl,
            Pat = encryptedPat,
            JiraProjectTag = request.JiraProjectTag,
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            Email = request.Email,
            IsActive = true
        };

        var result = _timeTrackDbContext.JiraSettings.Add(jiraSetting);
        await _timeTrackDbContext.SaveChanges(cancellationToken);

        return JiraSettingDto.Of(result.Entity);
    }

    internal sealed record Command(
        string Url,
        string Pat,
        string JiraProjectTag,
        string? Email,
        int ProjectId,
        int UserId
    ) : ICommand<JiraSettingDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Url).NotEmpty().Must(JiraUrlValidator.IsValid);
            RuleFor(x => x.Pat).NotEmpty();
            RuleFor(x => x.JiraProjectTag).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailType2Rule();
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
