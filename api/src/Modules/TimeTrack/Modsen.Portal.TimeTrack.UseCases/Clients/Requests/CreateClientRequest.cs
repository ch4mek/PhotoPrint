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
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class CreateClientRequest : ICommandHandler<CreateClientRequest.Command, CreatedClientDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly ICommunicationBus _communicationBus;
    private readonly IIsUserSalesManagerBusinessRule _isUserSalesManagerBusinessRule;

    public CreateClientRequest(
        ITimeTrackDbContext timeTrackDbContext,
        ITeamBoardContract teamBoardContract,
        ICommunicationBus communicationBus,
        IIsUserSalesManagerBusinessRule isUserSalesManagerBusinessRule)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _teamBoardContract = teamBoardContract;
        _communicationBus = communicationBus;
        _isUserSalesManagerBusinessRule  = isUserSalesManagerBusinessRule;
    }

    public async Task<CreatedClientDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isClientExistByName = await _timeTrackDbContext.Clients
            .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (isClientExistByName)
        {
            throw new EntityConflictException(PortalMessages.ClientWithTheSameNameExists);
        }

        await _isUserSalesManagerBusinessRule.IsCorrespondsToBusiness(request.SalesManagerId, cancellationToken);

        var countries = await _teamBoardContract.GetCountries(cancellationToken);

        var country = countries.FirstOrDefault(country => country.Id == request.RegisteredLocation);

        if (country is null)
        {
            throw new InvalidDataAppException(PortalMessages.RegisteredLocationNotFound);
        }

        var client = new Client
        {
            Name = request.Name,
            LegalEntity = request.LegalEntity,
            Comments = request.Comments,
            Color = request.Color,
            IsActive = true,
            RegisteredLocation = country.Name,
            SalesManagerId = request.SalesManagerId
        };

        _timeTrackDbContext.Clients.Add(client);

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        var notification = new ClientCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ClientId = client.Id,
            ClientName = client.Name,
            IsActive = client.IsActive,
            SalesManagerId = client.SalesManagerId
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new CreatedClientDto
        (
            Id: client.Id,
            Name: client.Name,
            LegalEntity: client.LegalEntity,
            Comments: client.Comments,
            Color: client.Color,
            RegisteredLocation: client.RegisteredLocation,
            SalesManagerId: client.SalesManagerId
        );
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
                .LongTextType()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.Color)
                .HexColorFormatRule();
        }
    }

    internal sealed record Command(
        string Name,
        string? LegalEntity,
        string? Comments,
        string? Color,
        int RegisteredLocation,
        int? SalesManagerId) : ICommand<CreatedClientDto>, ITransactionalRequest, ITimeTrackRequest;
}
