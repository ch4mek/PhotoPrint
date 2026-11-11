using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserLocationRequest : ICommandHandler<UpdateUserLocationRequest.Command, string>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICommunicationBus _communicationBus;

    public UpdateUserLocationRequest(IIdentityDbContext identityDbContext, ICommunicationBus communicationBus,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _communicationBus = communicationBus;
    }

    public async Task<string> Handle(Command request, CancellationToken cancellationToken)
    {
        var isEmployeeOrSysAdmin = _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator();

        var user = await _identityDbContext.Users.FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"User with id = {request.Id} does not exist");

        if (request.Id != _authenticatedUserContext.UserId)
        {
            if (isEmployeeOrSysAdmin)
            {
                throw new ActionNotAllowedException("You are not allowed to update another user location");
            }

            if ((_authenticatedUserContext.GetRoles().IsHeadOfDepartment() || _authenticatedUserContext.GetRoles().IsGroupManager()) &&
                _authenticatedUserContext.DepartmentId != user.DepartmentId)
            {
                throw new ActionNotAllowedException("You are not allowed to update user from another department");
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager() && _authenticatedUserContext.ResourceGroupId != user.ResourceGroupId)
            {
                throw new ActionNotAllowedException("You are not allowed to update user from another resource group");
            }
        }        

        user.Location = request.Location;

        var notification = new UserLocationUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = user.Id,
            Location = request.Location,
        };

        await _communicationBus.PublishAsync(
            notification,
            cancellationToken);

        await _identityDbContext.SaveChanges(cancellationToken);

        return request.Location.GetDescription();
    }

    internal sealed record Command(
        int Id,
        LocationType Location) : ICommand<string>, ITransactionalRequest, IIdentityRequest;


    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Location).IsInEnum()
                .WithMessage("Enter correct location");
        }
    }
}
