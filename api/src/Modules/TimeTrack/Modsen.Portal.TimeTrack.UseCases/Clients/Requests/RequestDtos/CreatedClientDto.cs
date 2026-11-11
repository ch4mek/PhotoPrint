namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

public record CreatedClientDto(int Id, string Name, string? LegalEntity, string? Comments, string? Color, string RegisteredLocation, int? SalesManagerId);

