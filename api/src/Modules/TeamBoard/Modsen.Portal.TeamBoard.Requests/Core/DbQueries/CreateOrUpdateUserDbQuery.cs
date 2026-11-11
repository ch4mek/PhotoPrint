using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface ICreateOrUpdateUserDbQuery
{
    Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken);
}

internal class CreateOrUpdateUserDbQuery : ICreateOrUpdateUserDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public CreateOrUpdateUserDbQuery(ITeamBoardDbContext teamBoardDbContext, IClockService clockService)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Include(x => x.RegisteredLocations)
            .ThenInclude(x => x.RegisteredLocation)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            user = new TeamBoardUser
            {
                UserId = request.UserId,
                IsActive = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedById = request.CreatedById
            };

            _teamBoardDbContext.TeamBoardUsers.Add(user);
        }

        if (!request.Position.IsNullOrEmpty())
        {
            user.Position = request.Position;
        }

        if (!request.Location.IsNullOrEmpty())
        {
            user.Location = request.Location;
        }

        if (request.RegisteredLocationId != null)
        {
            var currentRegisteredLocation = user.RegisteredLocations.FirstOrDefault(x => x.DateTo == null);

            if (currentRegisteredLocation?.RegisteredLocation?.Id != request.RegisteredLocationId)
            {
                if (currentRegisteredLocation != null)
                {
                    currentRegisteredLocation.DateTo = _clockService.TodayUtc();
                }

                user.RegisteredLocations.Add(new RegisteredLocationHistory
                {
                    RegisteredLocationId = request.RegisteredLocationId!.Value,
                    DateFrom = _clockService.TodayUtc(),
                });
            }
        }

        if (user.Position.IsIntern())
        {
            user.PersonalEmail = request.PersonalEmail ??
                (user.Email != null ? user.PersonalEmail : request.Email);
        }
        else
        {
            user.Email = request.Email;
            user.PersonalEmail = request.PersonalEmail ?? user.PersonalEmail;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FirstNameRu = request.FirstNameRu;
        user.LastNameRu = request.LastNameRu;
        user.PatronymicRu = request.PatronymicRu;
        
        user.CountryId = request.CountryId;
        user.DepartmentId = request.DepartmentId;
        user.AvatarFileId = request.AvatarFileId;
        user.PositionOrderWeight = request.PositionOrderWeight;
        user.ResourceGroupId = request.ResourceGroupId;
        user.HireDate = request.HireDate;
        user.CurrentLocation = request.CurrentLocation ?? user.CurrentLocation;

        user.MilitaryServiceStatus = request.MilitaryServiceStatus ?? user.MilitaryServiceStatus;
        user.IsGraduateJobPlacement = request.IsGraduateJobPlacement ?? user.IsGraduateJobPlacement;
        user.PersonalEmailSecond = request.PersonalEmailSecond ?? user.PersonalEmailSecond;
        user.PersonalPhone = request.PersonalPhone ?? user.PersonalPhone;
        user.Telegram = request.Telegram ?? user.Telegram;
        user.LinkedIn = request.LinkedIn ?? user.LinkedIn;
        user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }
}
