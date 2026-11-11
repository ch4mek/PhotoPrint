using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpSpecializationTemplate
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? CreatedById { get; set; }
    public bool CanBeUpdated { get; set; }
    public int HardSkillsCount { get; set; }

    internal static readonly Expression<Func<SpecializationTemplate, OdpSpecializationTemplate>> Map = st =>
        new OdpSpecializationTemplate
        {
            Id = st.Id,
            CreatedById = st.CreatedById,
            HardSkillsCount = st.SpecializationTemplateHardSkills.Count,
            Name = st.Name
        };
}
