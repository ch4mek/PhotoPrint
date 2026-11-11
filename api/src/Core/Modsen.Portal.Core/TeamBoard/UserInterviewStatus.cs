using System.ComponentModel;

namespace Modsen.Portal.Core.TeamBoard;

public enum UserInterviewStatus
{
    [Description("Waiting For Interview")]
    WaitingForInterview = 0,

    [Description("Waiting For Result")]
    WaitingForResult = 1,

    [Description("Failed")]
    Failed = 2,

    [Description("Success")]
    Success = 3,

    [Description("Started")]
    Started = 4,

    [Description("Not Started")]
    NotStarted = 5,

    [Description("Canceled")]
    Canceled = 6,

    [Description("No Feedback")]
    NoFeedback = 7,

    [Description("No Next Step Scheduled")]
    NoNextStepScheduled = 8,
}
