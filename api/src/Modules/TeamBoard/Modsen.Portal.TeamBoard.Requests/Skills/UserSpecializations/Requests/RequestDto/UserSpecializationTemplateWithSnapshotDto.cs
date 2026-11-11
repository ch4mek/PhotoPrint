using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto
{
    internal class UserSpecializationTemplateWithSnapshotDto
    {
        public int Id { get; set; }
        public string? SeniorityLevelId { get; set; }
        public string? SeniorityLevelName { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int SpecializationTemplateId { get; set; }
        public required string SpecializationTemplateName { get; set; }
        public IReadOnlyCollection<UserSnapshotCoreDto> SpecializationSnapshots { get; set; } = [];

        internal static readonly Expression<Func<UserSpecialization, UserSpecializationTemplateWithSnapshotDto>> Map = src => new UserSpecializationTemplateWithSnapshotDto
        {
            Id = src.Id,
            SeniorityLevelId = src.SeniorityLevelId,
            SeniorityLevelName = src.SeniorityLevel!.InternalName,
            SpecializationTemplateId = src.SpecializationTemplateId,
            SpecializationTemplateName = src.SpecializationTemplate!.Name,
            IsPrimary = src.IsPrimary,
            CreatedAtUtc = src.CreatedAtUtc
        };
    }
}
