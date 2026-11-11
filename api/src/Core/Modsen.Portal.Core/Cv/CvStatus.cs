using System.ComponentModel;

namespace Modsen.Portal.Core.Cv;

public enum CvStatus
{
    [Description("Draft")]
    Draft = 0,

    [Description("Sent")]
    Sent = 1,
}
