using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class UpdateClientRequest : ICommandHandler<UpdateClientRequest.Command, ClientDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly ICommunicationBus _communicationBus;
    private readonly IIsUserSalesManagerBusinessRule _isUserSalesManagerBusinessRule;

    public UpdateClientRequest(
        ITimeTrackDbContext timeTrackDbContext,
        ITeamBoardContract teamBoardContract,
        ICommunicationBus communicationBus,
        IIsUserSalesManagerBusinessRule isUserSalesManagerBusinessRule)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _teamBoardContract = teamBoardContract;
        _communicationBus = communicationBus;
        _isUserSalesManagerBusinessRule = isUserSalesManagerBusinessRule;
    }

    public async Task<ClientDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await _isUserSalesManagerBusinessRule.IsCorrespondsToBusiness(request.SalesManagerId, cancellationToken);

        var clientInfo = await _timeTrackDbContext.Clients
            .Where(client => client.Id == request.Id)
            .Select(client => new
            {
                IsProjectExist = client.Projects.Count != 0,
                Client = client
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ClientNotFound);

        var clientEntity = clientInfo.Client;

        if (!clientEntity.IsActive)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCantUpdateInactiveClient);
        }

        var isClientWithSameNameExist = await _timeTrackDbContext.Clients
            .Where(client => client.Name.ToLower() == request.Name.ToLower())
            .Where(client => client.Id != request.Id)
            .AnyAsync(cancellationToken);

        if (isClientWithSameNameExist)
        {
            throw new EntityConflictException(PortalMessages.ClientWithTheSameNameExists);
        }

        var countries = await _teamBoardContract.GetCountries(cancellationToken);
        var clientCountry = countries.FirstOrDefault(c => c.Id == request.RegisteredLocation);

        if (clientCountry is null)
        {
            throw new InvalidDataAppException(PortalMessages.RegisteredLocationNotFound);
        }

        clientEntity.Name = request.Name;
        clientEntity.LegalEntity = request.LegalEntity;
        clientEntity.Comments = request.Comments;
        clientEntity.RegisteredLocation = clientCountry.Name;
        clientEntity.Color = request.Color;
        clientEntity.SalesManagerId = request.SalesManagerId;

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        var notification = new ClientUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ClientId = clientEntity.Id,
            ClientName = clientEntity.Name,
            ClientLegalEntity = clientEntity.LegalEntity,
            IsActive = clientEntity.IsActive,
            SalesManagerId = clientEntity.SalesManagerId
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new ClientDto
        {
            Id = clientEntity.Id,
            Name = clientEntity.Name,
            LegalEntity = clientEntity.LegalEntity,
            RegisteredLocation = clientEntity.RegisteredLocation,
            Color = clientEntity.Color,
            Comments = clientEntity.Comments,
            HasActiveAssignements = null,
            HasActiveProject = null,
            IsActive = clientEntity.IsActive,
            BillableTrackedTimeInMinutes = clientEntity.BillableTrackedTime,
            FactTrackedTimeInMinutes = clientEntity.FactTrackedTime,
            IsProjectExist = clientInfo.IsProjectExist,
            SalesManagerId = clientEntity.SalesManagerId,
        };
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .ShortTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.LegalEntity)
               .ShortTextType()
               .SpacesOrPunctuationSymbolsRule()
               .When(x => !string.IsNullOrWhiteSpace(x.LegalEntity));

            RuleFor(x => x.Comments)
                .BeEmptyOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.Color)
                .HexColorFormatRule();
        }
    }

    internal sealed record Command(
        int Id,
        string Name,
        string? LegalEntity,
        string? Color,
        string? Comments,
        int RegisteredLocation,
        int? SalesManagerId) : ICommand<ClientDto>, ITransactionalRequest, ITimeTrackRequest;
}
