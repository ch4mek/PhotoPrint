using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class ActivateUserRequest : ICommandHandler<ActivateUserRequest.Command, UserDto>
{
    private readonly ICanIdentityUserBeActivatedBusinessRule _canIdentityUserBeActivatedBusinessRule;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLoader _userLoader;
    private readonly ICommunicationBus _communicationBus;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public ActivateUserRequest(
        ICanIdentityUserBeActivatedBusinessRule canIdentityUserBeActivatedBusinessRule,
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader,
        ICommunicationBus communicationBus,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canIdentityUserBeActivatedBusinessRule = canIdentityUserBeActivatedBusinessRule;
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
        _communicationBus = communicationBus;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.UserId, cancellationToken);

        var canBeActivatedResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, cancellationToken);

        if (!canBeActivatedResult.IsSuccess)
        {
            throw new ActionNotAllowedException(canBeActivatedResult.Reason!);
        }

        await _identityDbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.IsActive, true),
                cancellationToken);

        var notification = new UserActivatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = user.Id,
            ActivatedById = _authenticatedUserContext.GetRequiredUserId(),
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return await _userLoader.LoadAsync(request.UserId, cancellationToken);
    }

    internal sealed record Command(int UserId) : ICommand<UserDto>, ITransactionalRequest, IIdentityRequest;
}
