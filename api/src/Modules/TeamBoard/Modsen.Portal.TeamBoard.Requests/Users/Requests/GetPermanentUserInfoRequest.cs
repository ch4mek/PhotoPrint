using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests
{
    internal sealed class GetPermanentUserInfoRequest : IQueryHandler<GetPermanentUserInfoRequest.Query, PermanentUserInfoDto>
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;
        private readonly IIdentityContract _identityContract;
        private readonly ITimeTrackContract _timeTrackContract;

        public GetPermanentUserInfoRequest(
            ITeamBoardDbContext teamBoardDbContext,
            IIdentityContract identityContract,
            ITimeTrackContract timeTrackContract)
        {
            _teamBoardDbContext = teamBoardDbContext;
            _identityContract = identityContract;
            _timeTrackContract = timeTrackContract;
        }

        public async Task<PermanentUserInfoDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLower();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var user = await _teamBoardDbContext.TeamBoardUsers
                .Where(x => x.Email.ToLower() == email)
                .Select(user => new PermanentUserInfoDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FirstNameRu = user.FirstNameRu,
                    LastNameRu = user.LastNameRu,
                    PatronymicRu = user.PatronymicRu,
                    Email = user.Email,
                    HireDate = user.HireDate,

                    FirstProjectInCompanyStarted = user.UserProjects.Any()
                        ? user.UserProjects.Min(p => p.UserOnProjectFrom)
                        : null,

                    Contracts = user.Contracts.Select(x => new UserContractDto
                    {
                        DateFrom = x.ContractFrom,
                        DateTo = x.ContractTo,
                    }).ToList(),
                    ProfessionalActivityStartDate = user.CareerStartedAt,

                    Country = user.RegisteredLocations
                        .Where(x =>
                            x.DateFrom <= today &&
                            (x.DateTo == null || x.DateTo > today))
                        .Select(x => x.RegisteredLocation == null ? null : x.RegisteredLocation.Name)
                        .FirstOrDefault(),

                    EnglishLevel = user.EnglishLevel == null ? null : user.EnglishLevel.ShortName,
                    CurrentLocation = user.CurrentLocation,
                    LinkedIn = user.LinkedIn,
                    Location = user.Location,
                    DateOfBirth = user.DateOfBirth,
                    PersonalEmail = user.PersonalEmail,
                    PersonalEmailSecond = user.PersonalEmailSecond,
                    DepartamentId = user.DepartmentId,
                    IsGraduateJobPlacement = user.IsGraduateJobPlacement,
                    MilitaryServiceStatus = user.MilitaryServiceStatus,
                    PersonalPhone = user.PersonalPhone,
                    Position = user.Position,
                    ResourceGroupName = user.ResourceGroup == null ? null : user.ResourceGroup.ResourceGroupName,
                    Telegram = user.Telegram,

                    SeniorityLevel = user.UserSpecializations
                        .Where(specialization => specialization.IsPrimary)
                        .Select(specialization => specialization.SeniorityLevel != null
                            ? specialization.SeniorityLevel.InternalName
                            : null)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

            user.Managers = await GetManagers(user, cancellationToken);

            return user;
        }

        private async Task<IReadOnlyCollection<ManagerDto>?> GetManagers(PermanentUserInfoDto user, CancellationToken cancellationToken)
        {
            var userManagers = new List<ManagerDto>();

            var projectManagerIds = await _timeTrackContract.GetProjectManagerIdsByUserIdAsync(user.UserId, cancellationToken);

            if (user.Position.IsEmployee() || user.Position.IsSystemAdministrator() || user.Position.IsGroupManager())
            {
                var managerIds = await _identityContract.GetHigherManagerIds(user.UserId, cancellationToken);

                userManagers.AddRange(await _teamBoardDbContext.TeamBoardUsers
                    .Where(x => managerIds.Contains(x.UserId))
                    .Select(manager => new ManagerDto
                    {
                        Position = manager.Position,
                        FirstName = manager.FirstName,
                        LastName = manager.LastName,
                        FirstNameRu = manager.FirstNameRu,
                        LastNameRu = manager.LastNameRu,
                    })
                    .ToListAsync(cancellationToken));

                if (projectManagerIds != null && projectManagerIds.Count != 0)
                {
                    userManagers.AddRange(await _teamBoardDbContext.TeamBoardUsers
                        .Where(x => projectManagerIds.Contains(x.UserId))
                        .Select(manager => new ManagerDto
                        {
                            Position = "ProjectManager",
                            FirstName = manager.FirstName,
                            LastName = manager.LastName,
                            FirstNameRu = manager.FirstNameRu,
                            LastNameRu = manager.LastNameRu,
                        })
                        .ToListAsync(cancellationToken));
                }
            }

            if (user.Position.IsHeadOfDepartment())
            {
                var userChiefId = await _identityContract.GetUserChiefIdAsync(user.UserId, cancellationToken);

                if (userChiefId.HasValue)
                {
                    var chief = await _teamBoardDbContext.TeamBoardUsers
                        .Where(x => x.UserId == userChiefId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (chief != null)
                    {
                        userManagers.Add(new ManagerDto
                        {
                            Position = chief.Position,
                            FirstName = chief.FirstName,
                            LastName = chief.LastName,
                            FirstNameRu = chief.FirstNameRu,
                            LastNameRu = chief.LastNameRu,
                        });
                    }
                }
            }

            if (user.Position.IsCto())
            {
                var ceo = await _teamBoardDbContext.TeamBoardUsers
                    .Where(x => x.Position == PortalRoleNames.Ceo)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ceo != null)
                {
                    userManagers.Add(new ManagerDto
                    {
                        Position = ceo.Position,
                        FirstName = ceo.FirstName,
                        LastName = ceo.LastName,
                        FirstNameRu = ceo.FirstNameRu,
                        LastNameRu = ceo.LastNameRu,
                    });
                }
            }

            return userManagers;
        }

        internal sealed record Query(string Email) : IQuery<PermanentUserInfoDto>;

        internal sealed class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.Email)
                    .NotNull()
                    .NotEmpty();
            }
        }
    }
}
