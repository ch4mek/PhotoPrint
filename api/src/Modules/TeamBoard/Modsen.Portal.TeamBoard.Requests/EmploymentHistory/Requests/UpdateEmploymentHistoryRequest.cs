using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests;

internal sealed class UpdateEmploymentHistoryRequest
    : ICommandHandler<UpdateEmploymentHistoryRequest.Command, EmploymentHistoryDto>
{
    private readonly IIsTeamBoardUserExistWithHRAccessControlDbQuery _isTeamBoardUserExistingWithHrAccessControlDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public UpdateEmploymentHistoryRequest(
        IIsTeamBoardUserExistWithHRAccessControlDbQuery isTeamBoardUserExistingWithHrAccessControlDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService)
    {
        _isTeamBoardUserExistingWithHrAccessControlDbQuery = isTeamBoardUserExistingWithHrAccessControlDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task<EmploymentHistoryDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isTeamBoardUserExisting = await _isTeamBoardUserExistingWithHrAccessControlDbQuery
            .ExecuteAsync(request.UserId, cancellationToken);
        if (!isTeamBoardUserExisting)
        {
            throw new EntityNotFoundException("User is not found.");
        }

        return await UpdateEmploymentHistory(request, cancellationToken);
    }

    private async Task<EmploymentHistoryDto> UpdateEmploymentHistory(Command request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRequiredUserId() == request.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to edit your own employment history");
        }

        var teamBoardUser = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == request.UserId)
            .Include(user => user.Contracts)
            .FirstOrDefaultAsync(cancellationToken);

        teamBoardUser!.CareerStartedAt = request.ProfessionalActivityStartDate;
        teamBoardUser.UpdatedById = _authenticatedUserContext.UserId;

        if (teamBoardUser.Position.IsIntern())
        {
            teamBoardUser.NdaEndDate = request.NdaEndDate;
            teamBoardUser.NdaStartDate = request.NdaStartDate;
        }

        var currentContract = teamBoardUser.Contracts
            .FirstOrDefault(contract =>
                contract.ContractFrom <= _clockService.TodayUtc() &&
                (contract.ContractTo == null || contract.ContractTo >= _clockService.TodayUtc()));

        Validate(request, teamBoardUser, currentContract);

        if (currentContract != null)
        {
            currentContract.FireDate = request.FireDate;
        }

        _teamBoardDbContext.TeamBoardUsers.Update(teamBoardUser);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var internShipDetails = await _teamBoardDbContext.InternshipDetails
            .Where(x => x.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        var shouldDisplayNda = internShipDetails != null && internShipDetails.Type == InternshipType.NDA;

        return new EmploymentHistoryDto
        {
            ProfessionalActivityStartDate = teamBoardUser.CareerStartedAt,
            HireDate = teamBoardUser.HireDate,
            FireDate = currentContract?.FireDate,
            NdaStartDate = shouldDisplayNda ? teamBoardUser.NdaStartDate : null,
            NdaEndDate = shouldDisplayNda ? teamBoardUser.NdaEndDate : null,
            ShouldDisplayNda = shouldDisplayNda
        };
    }

    private static void Validate(Command request, TeamBoardUser teamBoardUser, UserContract? currentContract)
    {
        if (request.FireDate == null)
        {
            return;
        }

        if (request.FireDate < teamBoardUser.HireDate)
        {
            throw new InvalidDataAppException("Fire date cannot be earlier than Hire date");
        }

        if (currentContract == null)
        {
            throw new InvalidDataAppException("Fire date cannot be edited as there is no current contract");
        }

        if (request.FireDate <= currentContract.ContractFrom || request.FireDate > currentContract.ContractTo)
        {
            throw new InvalidDataAppException("Fire date must be later than Hire date and Start date of the current contract, and earlier than End date of the current contract");
        }
    }

    internal sealed record Command(
        int UserId,
        DateOnly? ProfessionalActivityStartDate,
        DateOnly? FireDate,
        DateOnly? NdaStartDate = null,
        DateOnly? NdaEndDate = null) : ICommand<EmploymentHistoryDto>, ITransactionalRequest;
}
