using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class CreateClientOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? LegalEntityName { get; set; }
    public string? Comments { get; set; }
    public string? Color { get; set; }
    public string? RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }

    internal static CreateClientOutput Of(CreatedClientDto source)
    {
        return new CreateClientOutput
        {
            Id = source.Id,
            Name = source.Name,
            LegalEntityName = source.LegalEntity,
            Comments = source.Comments,
            Color = source.Color,
            RegisteredLocation = source.RegisteredLocation,
            SalesManagerId = source.SalesManagerId
        };
    }
}
