namespace Modsen.Portal.TimeTrack.UseCases.Queries.Core.Dto;

internal sealed class AssignmentRow
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsActive { get; set; }
}
