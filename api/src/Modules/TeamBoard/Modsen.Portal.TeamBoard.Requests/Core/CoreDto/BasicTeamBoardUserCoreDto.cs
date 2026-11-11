using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal class BasicTeamBoardUserCoreDto
{
    public int UserId { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? Position { get; set; }
    public bool? IsActive { get; set; }
    public string? FullName { get; set; }
    public int? CreatedById { get; set; }


    public static BasicTeamBoardUserCoreDto? Of(TeamBoardUser? source)
    {
        return source is null
            ? null
            : new BasicTeamBoardUserCoreDto
            {
                UserId = source.UserId,
                DepartmentId = source.DepartmentId,
                ResourceGroupId = source.ResourceGroupId,
                Position = source.Position!,
                IsActive = source.IsActive,
                FullName = $"{source.LastName} {source.FirstName}",
                CreatedById = source.CreatedById,
            };
    }

    public static Expression<Func<TeamBoardUser, BasicTeamBoardUserCoreDto>> ExpressionOf => source =>
        new BasicTeamBoardUserCoreDto
        {
            UserId = source.UserId,
            DepartmentId = source.DepartmentId,
            ResourceGroupId = source.ResourceGroupId,
            Position = source.Position!,
            IsActive = source.IsActive,
            FullName = $"{source.LastName} {source.FirstName}",
            CreatedById = source.CreatedById
        };
}
