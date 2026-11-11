using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests;

internal sealed class DeleteInterviewRequest : ICommandHandler<DeleteInterviewRequest.Command, int>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IInterviewsAuthenticationFilter _interviewsAuthenticationFilter;

    public DeleteInterviewRequest(
        IInterviewsAuthenticationFilter interviewsAuthenticationFilter,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _interviewsAuthenticationFilter = interviewsAuthenticationFilter;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var interview = await _interviewsAuthenticationFilter.Apply(_teamBoardDbContext.UserInterviews)
            .Where(x => x.IsActive)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Interview not found");

        interview.IsActive = false;
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return interview.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull().NotEmpty();
        }
    }
}
