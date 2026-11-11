using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto
{
    internal class UserInterviewDto
    {
        public required int Id { get; set; }

        public required int UserId { get; set; }

        public required string Specialization { get; set; }

        public DateOnly? UserInterviewDate { get; set; }
        public TimeOnly? UserInterviewFromUtc { get; set; }

        public string? ClientName { get; set; }
        public UserInterviewStatus? Status { get; set; }
        public UserInterviewType? InterviewType { get; set; }

        public int? SalesResponsibleUserId { get; set; }

        public string? JiraTask { get; set; }
        public string? Notes { get; set; }
        public string? ReasonForRefusal { get; set; }

        public Guid GroupId { get; set; }
    }
}
