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

internal sealed class CreateUserSalaryListUseCase
    : ICommandHandler<CreateUserSalaryListUseCase.Command, List<SalaryDetailsDto>>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IIsSalaryFromDateValidBusinessRule _isSalaryFromDateValidBusinessRule;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserSalaryComposer _salaryComposer;


    public CreateUserSalaryListUseCase(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IIsSalaryFromDateValidBusinessRule isSalaryFromDateValidBusinessRule,
        IUserSalaryComposer salaryComposer)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _isSalaryFromDateValidBusinessRule = isSalaryFromDateValidBusinessRule;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _authenticatedUserContext = authenticatedUserContext;
        _salaryComposer = salaryComposer;
    }

    public async Task<List<SalaryDetailsDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        var authenticatedUserId = _authenticatedUserContext.UserId;

        foreach (var salaryDto  in request.Input)
        {
            var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule
                .IsCorrespondsToBusiness(salaryDto.UserId, cancellationToken);
            if (!canBeRetrieved)
            {
                throw new ActionNotAllowedException();
            }

            var isDateInvalid = await _isSalaryFromDateValidBusinessRule.IsInvalid(null, salaryDto.UserId,
                salaryDto.Date,
                cancellationToken);

            if (isDateInvalid)
            {
                throw new EntityConflictException(PortalMessages.SalaryDateConflicts);
            }
        }

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.UsdCurrencyId);

        var createUserList = request.Input.ConvertAll(salaryDto => new CreateUserSalaryCoreDto
        (
            UserId : salaryDto.UserId,
            SalaryFrom : salaryDto.Date,
            SalaryUsd : salaryDto.SalaryUsd,
            SalaryInCurrency : salaryDto.SalaryUsd,
            SalaryCurrencyId : settings.UsdCurrencyId,
            CreatedById : authenticatedUserId,
            CreatedAtUtc : DateTime.UtcNow,
            Source: request.Source ?? UserSalarySource.Unknown,
            ConversationId: salaryDto.ConversationId
        ));

        var result = await _salaryComposer.ComposeAsync(createUserList, cancellationToken);

        var resultDto = result.ConvertAll(coreDto => new SalaryDetailsDto
        (
            Id: coreDto.Id,
            UserId : coreDto.UserId,
            SalaryFrom : coreDto.SalaryFrom,
            SalaryUsd : coreDto.SalaryUsd
        ));

        return resultDto;
    }

    internal sealed record Command(List<CreateSalaryDto> Input, UserSalarySource? Source)
        : ICommand<List<SalaryDetailsDto>>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleForEach(command => command.Input).SetValidator(new CreateSalaryDto.Validator());
        }
    }
}
