using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;

internal sealed class UpdateUserSalaryRequest : ICommandHandler<UpdateUserSalaryRequest.Command, SalaryDetailsDto>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IIsSalaryFromDateValidBusinessRule _isSalaryFromDateValid;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly ISalaryEncryptor _salaryEncryptor;
    private readonly IUserSalaryLoader _salaryLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public UpdateUserSalaryRequest(ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IIsSalaryFromDateValidBusinessRule isSalaryFromDateValid,
        ITeamBoardDbContext teamboardDbContext,
        ISalaryEncryptor salaryEncryptor,
        IUserSalaryLoader salaryLoader,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _isSalaryFromDateValid = isSalaryFromDateValid;
        _teamboardDbContext = teamboardDbContext;
        _salaryEncryptor = salaryEncryptor;
        _salaryLoader = salaryLoader;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<SalaryDetailsDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        if (_authenticatedUserContext.UserId == request.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to update your own salary");
        }

        var isDateInvalid = request.Date != null &&
            await _isSalaryFromDateValid.IsInvalid(
                request.Id,
                request.UserId,
                request.Date.Value,
                cancellationToken);

        if (isDateInvalid)
        {
            throw new EntityConflictException(PortalMessages.SalaryDateConflicts);
        }

        var userSalary = await _salaryLoader.LoadAsync(request.Id, cancellationToken)
            ?? throw new ActionNotAllowedException(PortalMessages.UserSalaryNotFound);

        var canChangeSalary = await _canTeamBoardUserBeRetrievedBusinessRule
            .IsCorrespondsToBusiness(userSalary.UserId, cancellationToken);
        if (!canChangeSalary)
        {
            throw new ActionNotAllowedException();
        }

        var salaryDto = await UpdateSalaryAsync(request, userSalary, cancellationToken);

        return salaryDto;
    }

    private async Task<SalaryDetailsDto> UpdateSalaryAsync(UpdateSalaryDto request, UserSalaryCoreDto userSalary,
        CancellationToken cancellationToken)
    {
        var salaryFrom = request.Date ?? userSalary.SalaryFrom;
        var salaryUsd = request.SalaryUsd ?? userSalary.SalaryUsd;
        var salaryInCurrency = request.SalaryUsd ?? userSalary.SalaryUsd;

        var userSalaryDetails = await _teamboardDbContext.UserSalaries
            .FirstOrDefaultAsync(salary => salary.Id == request.Id, cancellationToken);

        var reqSalaryFrom = salaryFrom == DateOnly.MaxValue
            ? salaryFrom.AddDays(-1)
            : salaryFrom;

        userSalaryDetails!.Id = request.Id;
        userSalaryDetails.SalaryFrom = reqSalaryFrom;
        userSalaryDetails.SalaryUsd = _salaryEncryptor.Encrypt(salaryUsd)!;
        userSalaryDetails.SalaryInCurrency = _salaryEncryptor.Encrypt(salaryInCurrency)!;
        userSalaryDetails.CreatedById = userSalary.CreatedById;
        userSalaryDetails.CreatedAtUtc = userSalary.CreatedAtUtc;

        _teamboardDbContext.UserSalaries.Update(userSalaryDetails);

        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return new SalaryDetailsDto(
            Id: userSalaryDetails.Id,
            UserId: userSalaryDetails.UserId,
            SalaryFrom: userSalaryDetails.SalaryFrom,
            SalaryUsd: _salaryEncryptor.Decrypt(userSalaryDetails.SalaryUsd));
    }

    internal sealed record Command(UpdateSalaryDto RequestDto)
        : ICommand<SalaryDetailsDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new UpdateSalaryDto.Validator());
        }
    }
}
