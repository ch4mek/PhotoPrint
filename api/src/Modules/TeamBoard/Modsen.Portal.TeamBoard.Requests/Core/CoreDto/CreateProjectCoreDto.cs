using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal record CreateProjectCoreDto(
    int Id,
    bool IsActive,
    string ProjectName,
    string ClientName,
    int? CreatorId,
    bool IsPublic,
    string? Color)
{
    public TeamBoardProject ToDatabaseEntity(DateTime updatedAtUtc)
    {
        return new TeamBoardProject
        {
            Id = Id,
            IsActive = IsActive,
            IsPublic = IsPublic,
            ProjectName = ProjectName,
            ClientName = ClientName,
            Color = Color,
            UpdatedById = CreatorId,
            UpdatedAtUtc = updatedAtUtc
        };
    }
}
