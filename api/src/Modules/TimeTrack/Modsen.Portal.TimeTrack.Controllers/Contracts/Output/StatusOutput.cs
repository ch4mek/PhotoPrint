using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public record StatusOutput(int UserId, TimesheetSubmitStatusType? Status);
