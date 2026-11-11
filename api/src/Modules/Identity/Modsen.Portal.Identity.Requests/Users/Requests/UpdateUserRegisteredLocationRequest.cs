using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserRegisteredLocationRequest : ICommandHandler<UpdateUserRegisteredLocationRequest.Command, RegisteredLocationDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICommunicationBus _communicationBus;
    private readonly ITeamBoardContract _teamBoardContract;

    public UpdateUserRegisteredLocationRequest(
        IIdentityDbContext identityDbContext,
        ICommunicationBus communicationBus,
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardContract teamBoardContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _communicationBus = communicationBus;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<RegisteredLocationDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRequiredUserId() == request.Id)
        {
            throw new ActionNotAllowedException("You are not allowed to edit your own registered location");
        }

        var user = await _identityDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id = {request.Id} does not exist");

        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
                user.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException("You are not allowed to update user from another department");
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
                user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                throw new ActionNotAllowedException("You are not allowed to update user from another resource group");
            }
        }

        var countries = await _teamBoardContract.GetCountries(cancellationToken);

        if (countries.All(x => x.Id != request.RegisteredLocationId))
        {
            throw new InvalidDataAppException($"Country with id: {request.RegisteredLocationId} is not found");
        }

        user.RegisteredLocationId = request.RegisteredLocationId;
        user.CurrentLocation = request.CurrentLocation;
        user.Location = request.Location;

        var notification = new UserRegisteredLocationUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = user.Id,
            RegisteredLocationId = request.RegisteredLocationId,
            CurrentLocation = request.CurrentLocation,
            Location = request.Location
        };

        await _communicationBus.PublishAsync(
            notification,
            cancellationToken);

        await _identityDbContext.SaveChanges(cancellationToken);

        var registeredLocation = countries.FirstOrDefault(x => x.Id ==  request.RegisteredLocationId);

        return new RegisteredLocationDto
        {
            RegisteredLocationId = registeredLocation?.Id ?? 0,
            RegisteredLocationName = registeredLocation?.Name,
            CurrentLocation = user.CurrentLocation,
            Location = user.Location
        };
    }

    internal sealed record Command(
        int Id,
        int RegisteredLocationId,
        string? CurrentLocation,
        LocationType Location) : ICommand<RegisteredLocationDto>, ITransactionalRequest, IIdentityRequest {
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            When(x => x.CurrentLocation is not null, () =>
                RuleFor(x => x.CurrentLocation)
                    .AlphabeticWithHyphensSpaces()
                    .SpacesOrPunctuationSymbolsRule());

            RuleFor(x => x.Location).IsInEnum()
                .WithMessage("Enter correct location");
        }
    }
}
