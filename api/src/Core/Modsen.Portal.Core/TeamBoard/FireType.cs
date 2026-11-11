using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum FireType
{
    [Description("Voluntarily")]
    Voluntarily = 0,

    [Description("Not voluntarily")]
    NotVoluntarily = 1,

    [Description("By mutual agreement")]
    ByMutualAgreement = 2
}
