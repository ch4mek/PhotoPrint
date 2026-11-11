using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class UpdateClientStatusRequest : ICommandHandler<UpdateClientStatusRequest.Command, ClientStatusDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly ICommunicationBus _communicationBus;

    public UpdateClientStatusRequest(ITimeTrackDbContext timeTrackDbContext, ICommunicationBus communicationBus)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _communicationBus = communicationBus;
    }

    public async Task<ClientStatusDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var client = await _timeTrackDbContext.Clients
            .Include(client => client.Projects)
            .FirstOrDefaultAsync(client => client.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ClientNotFound);

        if (!request.IsActive && !client.IsActive)
        {
            throw new EntityConflictException(PortalMessages.ClientAlreadyDeleted);
        }

        if (!request.IsActive && client.Projects.Any(project => project.IsActive))
        {
            return new ClientStatusDto
            {
                IsSuccessful = false
            };
        }

        client.IsActive = request.IsActive;

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        var notification = new ClientUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            ClientId = client.Id,
            ClientName = client.Name,
            ClientLegalEntity = client.LegalEntity,
            IsActive = client.IsActive,
            SalesManagerId = client.SalesManagerId
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new ClientStatusDto
        {
            IsSuccessful = true
        };
    }

    internal sealed record Command(int Id, bool IsActive) : ICommand<ClientStatusDto>, ITimeTrackRequest, ITransactionalRequest;
}
