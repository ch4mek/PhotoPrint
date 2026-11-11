using System.Linq.Expressions;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal class OdpCvDto
{
    public int Id { get; set; }
    public required string CvName { get; set; }
    public string? UserFullName { get; set; }
    public string? UserFullNameRu { get; set; }
    public int UserId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserDepartmentId { get; set; }
    public CvStatus Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public bool IsVisibleToUser { get; set; }
    public string? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientId { get; set; }
    public string? ClientName { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    internal static readonly Expression<Func<CvEntity, OdpCvDto>> MapCv =
        entity => new OdpCvDto
        {
            Id = entity.Id,
            CvName = entity.CvName,
            Status = entity.Status,
            StatusOrderWeight = entity.StatusOrderWeight,
            IsVisibleToUser = entity.IsVisibleToUser,
            CreatedById = entity.CreatedById,
            CreatedAtUtc = entity.CreatedAtUtc,

            UserId = entity.UserId,
            UserFullName = entity.UserLastName + " " + entity.UserFirstName,
            UserFullNameRu = entity.CvUser!.LastNameRu == null || entity.CvUser.FirstNameRu == null
                ? null
                : entity.CvUser.LastNameRu + " " + entity.CvUser.FirstNameRu,
            UserDepartmentId = entity.CvUser.DepartmentId,
            UserResourceGroupId = entity.CvUser.ResourceGroupId,

            ProjectId = entity.ProjectId,
            ProjectName = entity.CvProject != null ? entity.CvProject.Name : null,

            ClientId = entity.ClientId,
            ClientName = entity.CvClient != null ? entity.CvClient.Name : null
        };
}
