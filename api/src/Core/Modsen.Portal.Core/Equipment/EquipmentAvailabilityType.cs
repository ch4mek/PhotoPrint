using System.ComponentModel;

namespace Modsen.Portal.Core.Equipment;

public enum EquipmentAvailabilityType
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Available")]
    Available = 1,

    [Description("Issued")]
    Issued = 2,

    [Description("Broken")]
    Broken = 3,

    [Description("Written off")]
    Writtenoff = 4,

    [Description("Renovation")]
    Renovation = 5
}
