using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal record ProjectUserCoreDto(
    int Id,
    int UserId,
    int ProjectId,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo,
    bool IsTimeOnProjectBillable,
    string SeniorityLevel,
    decimal Fte,
    string UserFirstName,
    string UserLastName,
    string ProjectName,
    string? SeniorityLevelId)
{
    public static ProjectUserCoreDto? Of(ProjectUser? source)
    {
        return source is null
            ? null
            : new ProjectUserCoreDto(
                Id: source.Id,
                UserId: source.UserId,
                ProjectId: source.ProjectId,
                UserOnProjectFrom: source.UserOnProjectFrom,
                UserOnProjectTo: source.UserOnProjectTo,
                IsTimeOnProjectBillable: source.IsTimeOnProjectBillable,
                SeniorityLevel: source.UserOnProjectSeniorityLevel!.InternalName,
                Fte: source.Fte,
                UserFirstName: source.User!.FirstName,
                UserLastName: source.User.LastName,
                ProjectName: source.Project!.ProjectName,
                SeniorityLevelId: source.UserOnProjectSeniorityLevelId);
    }
}
