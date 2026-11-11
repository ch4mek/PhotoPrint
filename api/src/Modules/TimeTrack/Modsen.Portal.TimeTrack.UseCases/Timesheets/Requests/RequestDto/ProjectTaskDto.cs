namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto
{
    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public required string TaskName { get; set; }
        public required string ProjectName { get; set; }
        public int FactTrackedTime { get; set; }
    }
}
