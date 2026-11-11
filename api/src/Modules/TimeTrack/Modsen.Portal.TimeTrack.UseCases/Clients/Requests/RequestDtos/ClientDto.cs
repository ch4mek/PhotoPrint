using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

internal class ClientDto
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public string? LegalEntity { get; set; }

    public string? Color { get; set; }

    public string? Comments { get; set; }
    public int? FactTrackedTimeInMinutes { get; set; }
    public int? BillableTrackedTimeInMinutes { get; set; }

    public bool IsActive { get; set; }

    public bool IsProjectExist { get; set; }

    public bool? HasActiveProject { get; set; }
    public bool? HasActiveAssignements { get; set; }
    public string? RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }
    public string? SalesManagerFullName { get; set; }

    public static ClientDto Of(Client source)
    {
        return new ClientDto
        {
            Id = source.Id,
            Name = source.Name,
            LegalEntity = source.LegalEntity,
            RegisteredLocation = source.RegisteredLocation,
            Color = source.Color,
            Comments = source.Comments,
            HasActiveAssignements = null,
            HasActiveProject = null,
            IsActive = source.IsActive,
            BillableTrackedTimeInMinutes = source.BillableTrackedTimeInMinutes,
            FactTrackedTimeInMinutes = source.FactTrackedTimeInMinutes,
            IsProjectExist = source.IsProjectExist,
            SalesManagerId = source.SalesManagerId
        };
    }

    public static ClientDto Of(PageableClientDbModel source)
    {
        return new ClientDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            Comments = source.Comments,
            IsActive = source.IsActive,
            IsProjectExist = source.IsProjectExist,
            HasActiveProject = source.HasActiveProject,
            FactTrackedTimeInMinutes = source.FactTrackedTimeInMinutes,
            BillableTrackedTimeInMinutes = source.BillableTrackedTimeInMinutes,
            RegisteredLocation = source.RegisteredLocation,
            SalesManagerId = source.SalesManagerId
        };
    }
}
