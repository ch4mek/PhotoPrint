using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum StudyForm
{
    [Description("Full-time")]
    FullTime,
    [Description("Correspodence")]
    Correspodence,
    [Description("Distance learning")]
    DistanceLearning,
    [Description("Evening")]
    Evening,
    [Description("Blending learning")]
    BlendingLearning
}
