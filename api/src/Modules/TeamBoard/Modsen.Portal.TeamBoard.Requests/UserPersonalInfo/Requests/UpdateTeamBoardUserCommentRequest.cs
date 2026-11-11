using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;

internal sealed class UpdateTeamBoardUserCommentRequest
    : ICommandHandler<UpdateTeamBoardUserCommentRequest.Command, UserCommentDto>
{
    private readonly IIsTeamBoardUserExistWithAccessControlDbQuery _isUserExistByUserIdDbQuery;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public UpdateTeamBoardUserCommentRequest(IIsTeamBoardUserExistWithAccessControlDbQuery isUserExistByUserIdDbQuery,
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _isUserExistByUserIdDbQuery = isUserExistByUserIdDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserCommentDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isTeamBoardUserExisting = await _isUserExistByUserIdDbQuery.ExecuteAsync(request.UserId,
            includeInterns: true,
            cancellationToken: cancellationToken);
        if (!isTeamBoardUserExisting)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.GetRequiredUserId() == request.UserId)
        {
            throw new ActionNotAllowedException();
        }

        var comment = await UpdateCommentAsync(request.UserId, request.Comment, cancellationToken);

        return new UserCommentDto(comment);
    }

    private async Task<string?> UpdateCommentAsync(int userId, string? comment, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers.FirstOrDefaultAsync(user => user.UserId == userId,
            cancellationToken);

        if (user!.Comments != comment)
        {
            user.Comments = comment;

            await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        }

        return user.Comments;
    }

    internal sealed record Command(int UserId, string? Comment) : ICommand<UserCommentDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Comment)
                .BeEmptyOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();
        }
    }
}
