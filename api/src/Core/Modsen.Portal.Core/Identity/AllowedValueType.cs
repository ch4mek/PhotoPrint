using System.ComponentModel;

namespace Modsen.Portal.Core.Identity;

public enum AllowedValueType
{
    [Description("String")]
    String = 0,
    [Description("Integer")]
    Integer = 1,
    [Description("HexColor")]
    HexColor = 2,
    [Description("DepartmentId")]
    DepartmentId = 3,
    [Description("CurrencyId")]
    CurrencyId = 4
}
