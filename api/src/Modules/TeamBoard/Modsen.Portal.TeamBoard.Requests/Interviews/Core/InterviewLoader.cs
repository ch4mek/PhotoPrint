using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Core
{
    internal interface IInterviewLoader
    {
        Task<UserInterviewDto?> Load(int entryId, CancellationToken cancellationToken);
    }

    internal class InterviewLoader : IInterviewLoader
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;

        public InterviewLoader(ITeamBoardDbContext teamBoardDbContext)
        {
            _teamBoardDbContext = teamBoardDbContext;
        }

        public async Task<UserInterviewDto?> Load(int entryId, CancellationToken cancellationToken)
        {
            return await _teamBoardDbContext.UserInterviews
                .AsNoTracking()
                .Where(x => x.Id == entryId)
                .Select(x => new UserInterviewDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Specialization = x.Specialization,
                    UserInterviewDate = x.UserInterviewDate,
                    UserInterviewFromUtc = x.UserInterviewFromUtc,
                    ClientName = x.ClientName,
                    Status = x.Status,
                    InterviewType = x.InterviewType,
                    SalesResponsibleUserId = x.SalesResponsibleUserId,
                    Notes = x.Notes,
                    JiraTask = x.JiraTask,
                    ReasonForRefusal = x.ReasonForRefusal,
                    GroupId = x.GroupId,
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
