using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ClientOutput
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public string? LegalEntityName { get; set; }
    public string? Color { get; set; }

    public string? Comments { get; set; }

    public bool IsActive { get; set; }

    public int? FactTrackedTimeInMinutes { get; set; }

    public int? BillableTrackedTimeInMinutes { get; set; }

    public bool IsProjectExist { get; set; }

    public bool? HasActiveProject { get; set; }
    public bool? HasActiveAssignements { get; set; }
    public string? RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }
    public string? SalesManagerFullName { get; set; }

    internal static ClientOutput? Of(ClientDto? source)
    {
        return source == null
            ? null
            : new ClientOutput()
        {
            Id = source.Id,
            Name = source.Name,
            LegalEntityName = source.LegalEntity,
            Color = source.Color,
            Comments = source.Comments,
            IsActive = source.IsActive,
            FactTrackedTimeInMinutes = source.FactTrackedTimeInMinutes,
            BillableTrackedTimeInMinutes = source.BillableTrackedTimeInMinutes,
            IsProjectExist = source.IsProjectExist,
            HasActiveProject = source.HasActiveProject,
            HasActiveAssignements = source.HasActiveAssignements,
            RegisteredLocation = source.RegisteredLocation,
            SalesManagerId =  source.SalesManagerId,
            SalesManagerFullName = source.SalesManagerFullName
        };
    }
}
