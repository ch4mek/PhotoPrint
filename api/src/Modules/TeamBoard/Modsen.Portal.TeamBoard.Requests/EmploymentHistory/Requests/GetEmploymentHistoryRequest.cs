using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests.RequestDto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests;

internal sealed class GetEmploymentHistoryRequest : IQueryHandler<GetEmploymentHistoryRequest.Query, EmploymentHistoryDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

    public GetEmploymentHistoryRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamBoardDbContext,
        ITimeTrackContract timeTrackContract,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
        _timeTrackContract = timeTrackContract;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<EmploymentHistoryDto> Handle(Query request, CancellationToken cancellationToken)
    {
        await ValidateUserAccess(request, cancellationToken);

        var projectUserInfo = await GetUserProjectInfoAsync(request.UserId, cancellationToken);

        return projectUserInfo;
    }

    private async Task ValidateUserAccess(Query request, CancellationToken cancellationToken)
    {
        var accessOptions = AccessOptions.Self
            | AccessOptions.Manager
            | AccessOptions.Hr
            | AccessOptions.Accountant
            | AccessOptions.Coordinator;

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            request.UserId, accessOptions, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }
    }

    private async Task<EmploymentHistoryDto> GetUserProjectInfoAsync(int userId, CancellationToken cancellationToken)
    {
        var result = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == userId)
            .Include(user => user.Contracts)
            .Include(user => user.UserProjects)
            .Select(user => new
            {                
                user.HireDate,
                user.CareerStartedAt,
                UserContracts = user.Contracts.Select(contract => new
                {
                    contract.ContractFrom,
                    contract.ContractTo,
                    contract.FireDate
                }),
                UserProjects = user.UserProjects.Select(projectUser =>
                    new { projectUser.UserId, projectUser.UserOnProjectFrom }),
                user.Position,
                user.NdaStartDate,
                user.NdaEndDate,
                LastInternshipType = user.InternshipDetails.OrderByDescending(i => i.StartDate).Select(i => i.Type).FirstOrDefault(),

            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        var latestContract = result?.UserContracts
            .OrderByDescending(x => x.ContractFrom)
            .FirstOrDefault();

        var userProjectsDates = result?.UserProjects
            .Where(project => project.UserId == userId)
            .Select(project => project.UserOnProjectFrom)
            .ToList();

        DateOnly? firstProjectDate = null;

        if (userProjectsDates is not null && userProjectsDates.Count > 0)
        {
            firstProjectDate = userProjectsDates.Min();
        }

        return new EmploymentHistoryDto
        {
            ProfessionalActivityStartDate = result?.CareerStartedAt,
            ContractEndDate = latestContract?.ContractTo,
            FirstProjectStartDate = firstProjectDate,
            HireDate = result?.HireDate,
            FireDate = latestContract?.FireDate,
            NdaStartDate = result?.NdaStartDate,
            NdaEndDate = result?.NdaEndDate,
            ShouldDisplayNda = result?.Position.IsIntern() == true && result?.LastInternshipType == InternshipType.NDA
        };
    }

    internal sealed record Query(int UserId) : IQuery<EmploymentHistoryDto>;
}
