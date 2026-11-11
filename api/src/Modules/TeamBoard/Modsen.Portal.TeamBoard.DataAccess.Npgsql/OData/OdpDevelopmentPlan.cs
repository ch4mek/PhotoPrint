using System.Linq.Expressions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpDevelopmentPlan
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? DeadlineDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DevelopmentPlanStatusType Status { get; set; }
    public string? Summary { get; set; }
    public string? Notes { get; set; }
    public int OrderWeight { get; set; }
    public int UserId { get; set; }
    public int? ConversationId { get; set; }

    internal static readonly Expression<Func<DevelopmentPlan, OdpDevelopmentPlan>> MapDevelopmentPlan =
        x => new OdpDevelopmentPlan
        {
            Id = x.Id,
            StartDate = x.StartDate,
            DeadlineDate = x.DeadlineDate,
            EndDate = x.EndDate,
            Status = x.Status,
            Summary = x.Summary,
            Notes = x.Notes,
            OrderWeight = x.OrderWeight,
            UserId = x.UserId,
            ConversationId = x.ConversationId
        };
}
