namespace Modsen.Portal.Utilities.Controllers.Recruitment;

public class RecruitmentFileOutput
{
    public required string FileId { get; init; }
    public long FileSize { get; init; }
    public string? FileName { get; init; }
}
