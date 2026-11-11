using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal record CreateProjectUserCoreDto(
    int Id,
    int UserId,
    int ProjectId,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo,
    bool IsTimeOnProjectBillable,
    decimal Fte,
    string UserOnProjectSeniorityLevelId)
{
    public ProjectUser ToDatabaseEntity()
    {
        return new ProjectUser
        {
            Id = Id,
            ProjectId = ProjectId,
            UserId = UserId,
            UserOnProjectFrom = UserOnProjectFrom,
            UserOnProjectTo = UserOnProjectTo,
            IsTimeOnProjectBillable = IsTimeOnProjectBillable,
            Fte = Fte,
            UserOnProjectSeniorityLevelId = UserOnProjectSeniorityLevelId,
        };
    }
}
