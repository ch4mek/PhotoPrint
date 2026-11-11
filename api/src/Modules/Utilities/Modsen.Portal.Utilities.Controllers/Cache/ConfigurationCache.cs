using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Contract;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.Utilities.Controllers.Configuration;

namespace Modsen.Portal.Utilities.Controllers.Cache;

public interface IConfigurationCache
{
    Task<ConfigurationOutput> GetConfiguration();
}

public class ConfigurationCache : IConfigurationCache
{
    private readonly IMerchContract _merchContract;
    private readonly IIdentityContract _identityContract;
    private readonly IEquipmentContract _equipmentContract;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IBookingContract _bookingContract;
    private readonly IMemoryCache _memoryCache;

    private const string _cacheKey = "__MMS_WEB_CONFIG";
    private static readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public ConfigurationCache(
        IIdentityContract identityContract,
        IEquipmentContract equipmentContract,
        ITeamBoardContract teamBoardContract,
        IBookingContract bookingContract,
        IMemoryCache memoryCache,
        IMerchContract merchContract)
    {
        _identityContract = identityContract;
        _equipmentContract = equipmentContract;
        _teamBoardContract = teamBoardContract;
        _bookingContract = bookingContract;
        _memoryCache = memoryCache;
        _merchContract = merchContract;
    }

    public async Task<ConfigurationOutput> GetConfiguration()
    {
        return await _memoryCache.GetOrCreateAsync(_cacheKey, _ => LoadConfiguration(), _cacheOptions)
            ?? await LoadConfiguration();
    }

    private async Task<ConfigurationOutput> LoadConfiguration()
    {
        var cancellationToken = CancellationToken.None;
        var seniorityLevelScope = SeniorityLevelScopes.HardSkill
            | SeniorityLevelScopes.UserSpecialization
            | SeniorityLevelScopes.ProjectUser
            | SeniorityLevelScopes.TeamBoardUser;

        var softSkillSeniorityLevelScope = SeniorityLevelScopes.SoftSkill;
        var generalSeniorityLevelScope = SeniorityLevelScopes.GeneralSeniority;

        var departmentValues = await _identityContract.GetDepartmentsClassificator(cancellationToken);
        var rolesValues = await _identityContract.GetRolesClassificator(cancellationToken);
        var equipmentTypeValues = await _equipmentContract.GetEquipmentTypes(cancellationToken);
        var officesValues = await _bookingContract.GetOfficesClassificator(cancellationToken);
        var hardSkillGroups = await _teamBoardContract.GetHardSkillGroups(cancellationToken);
        var seniorityLevels = await _teamBoardContract.GetSeniorityLevels(seniorityLevelScope, cancellationToken);
        var softSkillSeniorityLevels = await _teamBoardContract.GetSeniorityLevels(softSkillSeniorityLevelScope, cancellationToken);
        var englishLevels = await _teamBoardContract.GetEnglishLevels(cancellationToken);
        var generalSenioritySeniorityLevels = await _teamBoardContract.GetSeniorityLevels(generalSeniorityLevelScope, cancellationToken);
        var countries = await _teamBoardContract.GetCountries(cancellationToken);
        var merchSizes = await _merchContract.GetMerchSizes(cancellationToken);
        var merchStorages = await _merchContract.GetMerchStorages(cancellationToken);

        return new ConfigurationOutput
        {
            PersistentValues = new PersistentValues
            {
                LocationTypes = EnumExtensions.GetValues(LocationType.Unknown),
                JsStackTypes = EnumExtensions.GetValues(JsStackType.None),
                EquipmentLocationTypes = EnumExtensions.GetValues(EquipmentLocationType.Unknown),
                EquipmentAvailabilityTypes = EnumExtensions.GetValues(EquipmentAvailabilityType.Unknown),
                EquipmentBrokenStatuses = EnumExtensions.GetValues<EquipmentBrokenStatus>(),
                MoneyCurrencyTypes = EnumExtensions.GetValues(MoneyCurrencyType.Unknown),
                PermissionTypes = EnumExtensions.GetValues(PermissionType.None),
                PermissionResourceTypes = EnumExtensions.GetValues(PermissionResourceType.None),
                ExternalFeedbackTypes = EnumExtensions.GetValues(ExternalFeedbackType.Unknown),
                FireTypes = EnumExtensions.GetValues<FireType>(),
                StudyFormTypes = EnumExtensions.GetValues<StudyForm>(),
                EducationLevelTypes = EnumExtensions.GetValues<EducationLevel>(),
                MerchStorageStatuses = EnumExtensions.GetValues(StorageAvailabilityStatus.Unknown),
                MerchItemStatuses = EnumExtensions.GetValues<MerchItemStatus>(),
                AbsenceTypes = EnumExtensions.GetValues(exceptValues: InquiryType.Unknown),
                TrackingUnitTypes = EnumExtensions.GetValues<TrackingUnit>(),
                ActivityTypes = EnumExtensions.GetValues<ActivityType>(),
                AccrualTypes = EnumExtensions.GetValues<AccrualType>(),
                TransferTypes = EnumExtensions.GetValues<TransferType>(),
                MerchTransactionTypes = EnumExtensions.GetValues<MerchTransactionType>(),
            },
            ConfigurableValues = new ConfigurableValues
            {
                RoleTypes = rolesValues,
                DepartmentTypes = departmentValues,
                EquipmentTypes = equipmentTypeValues,
                Offices = officesValues.ToList(),
                HardSkillGroups = hardSkillGroups,
                SeniorityLevels = seniorityLevels,
                SoftSkillSeniorityLevels = softSkillSeniorityLevels,
                GeneralSenioritySeniorityLevels = generalSenioritySeniorityLevels,
                EnglishLevels = englishLevels,
                Countries = countries,
                MerchSizes = merchSizes.ToList(),
                MerchStorages = merchStorages.ToList()
            },
        };
    }
}
