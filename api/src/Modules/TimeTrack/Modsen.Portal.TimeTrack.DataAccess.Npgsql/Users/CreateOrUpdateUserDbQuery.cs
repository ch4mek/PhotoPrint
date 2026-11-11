using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users
{
    internal class CreateOrUpdateUserDbQuery : ICreateOrUpdateUserDbQuery
    {
        private readonly ITimeTrackDbContext _timeTrackDbContext;

        public CreateOrUpdateUserDbQuery(ITimeTrackDbContext timeTrackDbContext)
        {
            _timeTrackDbContext = timeTrackDbContext;
        }

        public async Task Execute(TimeTrackUser timeTrackUser, string? position, CancellationToken cancellationToken)
        {
            var user = await _timeTrackDbContext.TimeTrackUsers
                .FirstOrDefaultAsync(x => x.UserId == timeTrackUser.UserId, cancellationToken);
            if (user == null)
            {
                user = new DatabaseEntities.TimeTrackUser
                {
                    UserId = timeTrackUser.UserId,
                    IsActive = true,
                    FirstName = timeTrackUser.FirstName,
                    LastName = timeTrackUser.LastName,
                    Email = timeTrackUser.Email,
                    CreatedById = timeTrackUser.CreatedById,
                };

                _timeTrackDbContext.TimeTrackUsers.Add(user);
            }

            user.FirstName = timeTrackUser.FirstName;
            user.LastName = timeTrackUser.LastName;
            user.Email = timeTrackUser.Email;
            user.DepartmentId = timeTrackUser.DepartmentId;
            user.ResourceGroupId = timeTrackUser.ResourceGroupId;
            user.Color = timeTrackUser.Color;
            user.AvatarFileId = timeTrackUser.AvatarFileId;

            if (position != null)
            {
                user.Position = position;
            }

            await _timeTrackDbContext.SaveChanges(cancellationToken);
        }
    }
}
