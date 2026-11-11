using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.LabBoard.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;

internal sealed class GetInternshipRequest : IQueryHandler<GetInternshipRequest.Query, InternshipDto?>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetInternshipRequest(ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<InternshipDto?> Handle(Query request, CancellationToken cancellationToken)
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

        var internship = await _teamBoardDbContext.InternshipDetails
            .Where(i => i.UserId == request.UserId)
            .OrderByDescending(i => i.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (internship is null)
        {
            return null;
        }

        var specializations = await _teamBoardDbContext.InternshipSpecializations
            .Where(uis => uis.InternshipDetailId == internship.Id)
            .Include(s => s.InternshipSpecializationHardSkills)
                .ThenInclude(s => s.HardSkill)
            .Include(s => s.SeniorityLevel)
            .Include(s => s.SpecializationTemplate)
            .ToListAsync(cancellationToken);

        internship.InternshipSpecializations = specializations;

        return InternshipDto.Of(internship);
    }

    internal sealed record Query(int UserId) : IQuery<InternshipDto?>;
}
