using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.Composers
{
    internal interface ISpecializationSnapshotComposer
    {
        Task<int> CreateForDate(int userId, DateOnly date, CancellationToken cancellationToken);
        Task<int> Create(int userId, CancellationToken cancellationToken);
    }

    internal class SpecializationSnapshotComposer : ISpecializationSnapshotComposer
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly IClockService _clockService;
        public SpecializationSnapshotComposer(ITeamBoardDbContext teamBoardDbContext,
            IAuthenticatedUserContext authenticatedUserContext,
            IClockService clockService)
        {
            _teamBoardDbContext = teamBoardDbContext;
            _authenticatedUserContext = authenticatedUserContext;
            _clockService = clockService;
        }

        public async Task<int> CreateForDate(int userId, DateOnly date, CancellationToken cancellationToken)
        {
            var userSpecializations = await _teamBoardDbContext.UserSpecializations
                .Where(us => us.UserId == userId)
                .Include(us => us.SpecializationHardSkills)
                .ToListAsync(cancellationToken);

            var snapshot = new UserSkillsSnapshot
            {
                UserId = userId,
                SnapshotDate = date,
                CreatedById = _authenticatedUserContext.UserId,
                CreatedAtUtc = _clockService.UtcNow,
                Specializations = userSpecializations.Select(us => new UserSkillsSnapshotSpecialization
                {
                    SpecializationTemplateId = us.SpecializationTemplateId,
                    SeniorityLevelId = us.SeniorityLevelId,
                    UserSpecializationWasPrimary = us.IsPrimary,
                    HardSkills = us.SpecializationHardSkills.Select(hs => new UserSkillsSnapshotSpecializationHardSkill
                    {
                        HardSkillId = hs.HardSkillId,
                        SeniorityLevelId = hs.SeniorityLevelId
                    }).ToArray()
                }).ToArray()
            };

            _teamBoardDbContext.UserSkillsSnapshots.Add(snapshot);

            await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

            return snapshot.Id;
        }

        public async Task<int> Create(int userId, CancellationToken cancellationToken)
        {
            var lastUserSnapshot = await GetLastUserSnapshotByUserId(userId, cancellationToken);

            var todaysDate = DateOnly.FromDateTime(_clockService.UtcNow);

            if (lastUserSnapshot?.SnapshotDate == todaysDate)
            {
                await DeleteUserSnapshot(lastUserSnapshot, cancellationToken);
            }

            return await CreateForDate(userId, todaysDate, cancellationToken);
        }


        private async Task<UserSkillsSnapshot?> GetLastUserSnapshotByUserId(int userId, CancellationToken cancellationToken)
        {
            var userSnapshot = await _teamBoardDbContext.UserSkillsSnapshots
                .Where(us => us.UserId == userId && !us.IsDeleted)
                .OrderByDescending(us => us.SnapshotDate)
                .FirstOrDefaultAsync(cancellationToken);

            return userSnapshot;
        }

        private async Task DeleteUserSnapshot(UserSkillsSnapshot userSnapshot, CancellationToken cancellationToken)
        {

            userSnapshot.IsDeleted = true;

            _teamBoardDbContext.UserSkillsSnapshots.Update(userSnapshot);

            await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
