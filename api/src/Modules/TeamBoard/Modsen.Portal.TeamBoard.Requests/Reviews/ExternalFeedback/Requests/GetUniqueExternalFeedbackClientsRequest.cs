using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;

internal sealed class GetUniqueExternalFeedbackClientsRequest :
    IQueryHandler<GetUniqueExternalFeedbackClientsRequest.Query, List<string>>
{
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusiness;
    public GetUniqueExternalFeedbackClientsRequest(
        ITimeTrackContract timeTrackContract,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusiness)
    {
        _timeTrackContract = timeTrackContract;
        _canTeamBoardUserBeRetrievedBusiness = canTeamBoardUserBeRetrievedBusiness;
    }

    public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _canTeamBoardUserBeRetrievedBusiness.IsCorrespondsToBusiness(request.UserId, Portal.Core.AccessOptions.Manager, cancellationToken))
        {
            throw new ActionNotAllowedException();
        }

        return await _timeTrackContract.GetClientNamesFromProjectsUserIsAssignedTo(request.UserId, cancellationToken);
    }

    internal sealed record Query(int UserId) : IQuery<List<string>>;
}
