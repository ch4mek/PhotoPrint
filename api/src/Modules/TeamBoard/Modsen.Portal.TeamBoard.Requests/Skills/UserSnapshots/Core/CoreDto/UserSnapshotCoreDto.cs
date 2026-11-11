namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

public class UserSnapshotCoreDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public IReadOnlyCollection<UserSnapshotSpecializationCoreDto> UserSpecializations { get; set; } = [];
}
