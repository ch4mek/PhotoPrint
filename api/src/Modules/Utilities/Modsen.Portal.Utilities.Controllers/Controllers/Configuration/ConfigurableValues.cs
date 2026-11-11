using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Equipment.Contract.Models;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;

namespace Modsen.Portal.Utilities.Controllers.Configuration;

public class ConfigurableValues
{
    public IReadOnlyCollection<RoleClassificationDto> RoleTypes { get; set; } = [];
    public IReadOnlyCollection<OfficeClassificationDto> Offices { get; set; } = [];
    public IReadOnlyCollection<DepartmentClassificationDto> DepartmentTypes { get; set; } = [];
    public IReadOnlyCollection<EquipmentTypeClassificatorDto> EquipmentTypes { get; set; } = [];
    public IReadOnlyCollection<ShortHardSkillGroupOutput> HardSkillGroups { get; set; } = [];
    public IReadOnlyCollection<SeniorityLevelOutput> SeniorityLevels { get; set; } = [];
    public IReadOnlyCollection<SeniorityLevelOutput> SoftSkillSeniorityLevels { get; set; } = [];
    public IReadOnlyCollection<SeniorityLevelOutput> GeneralSenioritySeniorityLevels { get; set; } = [];
    public IReadOnlyCollection<EnglishLevelOutput> EnglishLevels { get; set; } = [];
    public IReadOnlyCollection<CountryOutput> Countries { get; set; } = [];
    public IReadOnlyCollection<MerchSizeDto> MerchSizes { get; set; } = [];
    public IReadOnlyCollection<MerchStorageDto> MerchStorages { get; set; } = [];
}
