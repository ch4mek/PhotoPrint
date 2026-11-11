using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;

internal sealed class UpdateUserSalaryListUseCase : ICommandHandler<UpdateUserSalaryListUseCase.Command, List<SalaryDetailsDto>>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IIsSalaryFromDateValidBusinessRule _isSalaryFromDateValidBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ISalaryEncryptor _salaryEncryptor;

    public UpdateUserSalaryListUseCase(
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IIsSalaryFromDateValidBusinessRule isSalaryFromDateValidBusinessRule,
        ITeamBoardDbContext teamBoardDbContext,
        ISalaryEncryptor salaryEncryptor)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _isSalaryFromDateValidBusinessRule = isSalaryFromDateValidBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
        _salaryEncryptor = salaryEncryptor;
    }

    public async Task<List<SalaryDetailsDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        foreach (var input in request.Input)
        {
            var canBeUpdated = await _canTeamBoardUserBeRetrievedBusinessRule
                .IsCorrespondsToBusiness(input.UserId, cancellationToken);
            if (!canBeUpdated)
            {
                throw new ActionNotAllowedException();
            }

            var isFromDateInvalid = input.Date != null && await _isSalaryFromDateValidBusinessRule
                .IsInvalid(input.Id, input.UserId, input.Date.Value, cancellationToken);
            if (isFromDateInvalid)
            {
                throw new EntityConflictException(PortalMessages.SalaryDateConflicts);
            }
        }

        return await UpdateSalariesAsync(request, cancellationToken);
    }

    private async Task<List<SalaryDetailsDto>> UpdateSalariesAsync(Command request, CancellationToken cancellationToken)
    {
        var ids = request.Input.Select(salaryDto => salaryDto.Id).ToList();

        var userSalaries = await _teamBoardDbContext.UserSalaries
            .Where(us => ids.Contains(us.Id))
            .ToListAsync(cancellationToken);

        var responses = new List<SalaryDetailsDto>();

        foreach (var req in request.Input)
        {
            var userSalary = userSalaries.FirstOrDefault(us => us.Id == req.Id);
            var reqSalaryFrom = req.Date == DateOnly.MaxValue ? req.Date.Value.AddDays(-1) : req.Date;

            userSalary!.SalaryFrom = reqSalaryFrom ?? userSalary.SalaryFrom;
            userSalary.SalaryUsd = _salaryEncryptor.Encrypt(req.SalaryUsd) ?? userSalary.SalaryUsd;
            userSalary.SalaryInCurrency = _salaryEncryptor.Encrypt(req.SalaryUsd) ?? userSalary.SalaryInCurrency;

            var response = new SalaryDetailsDto(
                Id: userSalary.Id,
                UserId: userSalary.UserId,
                SalaryFrom: userSalary.SalaryFrom,
                SalaryUsd: _salaryEncryptor.Decrypt(userSalary.SalaryUsd));

            responses.Add(response);
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return responses;
    }

    internal sealed record Command(List<UpdateSalaryDto> Input) : ICommand<List<SalaryDetailsDto>>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleForEach(command => command.Input).SetValidator(new UpdateSalaryDto.Validator());
        }
    }
}
