using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class UserProjectLookupOutput
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public required string ProjectName { get; set; }
    public bool? IsTimeOnProjectBillable { get; set; }
    public string? ClientName { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsActive { get; set; }

    public static UserProjectLookupOutput? Of(UserProjectLookupDto? source)
    {
        if (source == null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        return new UserProjectLookupOutput
        {
            Id = source.Id,
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            IsTimeOnProjectBillable = source.IsTimeOnProjectBillable,
            UserOnProjectFrom = source.UserOnProjectFrom,
            UserOnProjectTo = source.UserOnProjectTo,
            ClientName = source.ClientName,
            IsActive = today <= source.UserOnProjectFrom ||
                (today >= source.UserOnProjectFrom &&
                    (source.UserOnProjectTo == null || today <= source.UserOnProjectTo)),
        };
    }
}
