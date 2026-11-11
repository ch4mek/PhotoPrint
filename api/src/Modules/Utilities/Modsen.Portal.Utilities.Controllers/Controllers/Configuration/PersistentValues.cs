using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Utilities.Controllers.Configuration
{
    public class PersistentValues
    {
        public IReadOnlyCollection<EnumModel> LocationTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> JsStackTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> EquipmentLocationTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> EquipmentAvailabilityTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> EquipmentBrokenStatuses {  get; set; } = [];
        public IReadOnlyCollection<EnumModel> MoneyCurrencyTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> PermissionTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> PermissionResourceTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> ExternalFeedbackTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> FireTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> StudyFormTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> EducationLevelTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> MerchStorageStatuses { get; set; } = [];
        public IReadOnlyCollection<EnumModel> MerchItemStatuses { get; set; } = [];
        public IReadOnlyCollection<EnumModel> AbsenceTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> TrackingUnitTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> ActivityTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> AccrualTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> TransferTypes { get; set; } = [];
        public IReadOnlyCollection<EnumModel> MerchTransactionTypes { get; set; } = [];
    }
}
