using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;

internal sealed class CreateUserSalaryRequest
    : ICommandHandler<CreateUserSalaryRequest.Command, SalaryDetailsDto>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIsSalaryFromDateValidBusinessRule _isSalaryFromDateValidBusinessRule;
    private readonly IUserSalaryComposer _salaryComposer;

    public CreateUserSalaryRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor,
        IIsSalaryFromDateValidBusinessRule isSalaryFromDateValidBusinessRule,
        IUserSalaryComposer salaryComposer)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _isSalaryFromDateValidBusinessRule = isSalaryFromDateValidBusinessRule;
        _salaryComposer = salaryComposer;
    }

    public async Task<SalaryDetailsDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        if (_authenticatedUserContext.GetRequiredUserId() == request.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to create your own salary");
        }

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId,
            cancellationToken);
        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var isDateInvalid = await _isSalaryFromDateValidBusinessRule.IsInvalid(null, request.UserId,
            request.Date,
            cancellationToken);
        if (isDateInvalid)
        {
            throw new EntityConflictException(PortalMessages.SalaryDateConflicts);
        }

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.UsdCurrencyId);

        var salaryCoreDto = await _salaryComposer.ComposeAsync(
            new CreateUserSalaryCoreDto(
                UserId: request.UserId,
                SalaryFrom: request.Date,
                SalaryUsd: request.SalaryUsd,
                SalaryInCurrency: request.SalaryUsd,
                SalaryCurrencyId: settings.UsdCurrencyId,
                CreatedById: _authenticatedUserContext.UserId,
                CreatedAtUtc: DateTime.UtcNow,
                Source: command.Source ?? UserSalarySource.Unknown,
                ConversationId: request.ConversationId),
            cancellationToken);

        return SalaryDetailsDto.Of(salaryCoreDto);
    }

    internal sealed record Command(CreateSalaryDto RequestDto, UserSalarySource? Source)
        : ICommand<SalaryDetailsDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new CreateSalaryDto.Validator());
        }
    }
}
