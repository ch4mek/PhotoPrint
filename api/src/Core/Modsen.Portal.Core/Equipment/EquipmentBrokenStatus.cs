using System.ComponentModel;

namespace Modsen.Portal.Core.Equipment;

public enum EquipmentBrokenStatus
{
    [Description("Available")]
    None = 0,

    [Description("Broken")]
    Broken = 1,

    [Description("Renovation")]
    Renovation = 2,

    [Description("Written off")]
    Writtenoff = 3
}
