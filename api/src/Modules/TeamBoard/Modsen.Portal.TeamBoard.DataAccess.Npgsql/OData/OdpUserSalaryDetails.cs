using System.Linq.Expressions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserSalaryDetails
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public DateOnly SalaryFrom { get; set; }
    public required string SalaryUsd { get; set; }
    public UserSalarySource Source { get; set; }
    public int? ConversationId { get; set; }
    public bool IsCurrent { get; set; }

    internal static readonly Expression<Func<UserSalary, OdpUserSalaryDetails>> MapUserSalary =
        x => new OdpUserSalaryDetails
        {
            Id = x.Id,
            UserId = x.UserId,
            SalaryFrom = x.SalaryFrom,
            SalaryUsd = x.SalaryUsd,
            Source = x.Source,
            ConversationId = x.ConversationId,
            IsCurrent = x.Id == x.User!.SalaryDetails
                .Where(us => us.SalaryFrom <= DateOnly.FromDateTime(DateTime.UtcNow))
                .OrderByDescending(us => us.SalaryFrom)
                .ThenByDescending(us => us.CreatedAtUtc)
                .Select(us => us.Id)
                .FirstOrDefault()
        };
}
