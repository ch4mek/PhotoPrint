using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;

internal sealed class UpdatePersonalDataRequest : ICommandHandler<UpdatePersonalDataRequest.Command, PersonalDataDto>
{
    private readonly IIsTeamBoardUserExistWithHRAccessControlDbQuery _isUserExistWithHRAccessControlDbQuery;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IIdentityCrossWriterContract _identityCrossWriterContract;
    private readonly IArePersonalPhoneAndEmailsUniqueBusinessRule _arePersonalPhoneAndEmailsUniqueBusinessRule;

    public UpdatePersonalDataRequest(
        IIsTeamBoardUserExistWithHRAccessControlDbQuery isUserExistWithAccessControlDbQuery,
        ITeamBoardDbContext teamBoardDbContext,
        IIdentityCrossWriterContract identityCrossWriterContract,
        IArePersonalPhoneAndEmailsUniqueBusinessRule arePersonalPhoneAndEmailsUniqueBusinessRule)
    {
        _isUserExistWithHRAccessControlDbQuery = isUserExistWithAccessControlDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
        _identityCrossWriterContract = identityCrossWriterContract;
        _arePersonalPhoneAndEmailsUniqueBusinessRule = arePersonalPhoneAndEmailsUniqueBusinessRule;
    }

    public async Task<PersonalDataDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        var isTeamBoardUserExist = await _isUserExistWithHRAccessControlDbQuery.ExecuteAsync(request.UserId,
            cancellationToken);

        if (!isTeamBoardUserExist)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var currentUser = await _teamBoardDbContext.TeamBoardUsers
            .Where(u => u.UserId == request.UserId)
            .Select(u => new { u.Position, u.DepartmentId, u.Email })
            .FirstAsync(cancellationToken);

        await ValidateHeadOfDepartmentUniqueness(request.UserId, request.Position, request.DepartmentId,
            currentUser.Position, currentUser.DepartmentId, cancellationToken);

        var arePersonalUnique = await _arePersonalPhoneAndEmailsUniqueBusinessRule.IsCorrespondsToBusiness(
            new ArePersonalPhoneAndEmailsUniqueBusinessModel
            {
                UserId = request.UserId,
                PersonalEmail = request.PersonalEmail,
                PersonalEmailSecond = request.PersonalEmailSecond,
                PersonalPhone = request.PersonalPhone
            }, cancellationToken);

        if (!arePersonalUnique.IsSuccess)
        {
            throw new EntityConflictException(arePersonalUnique.Reason ?? string.Empty);
        }

        _ = await _teamBoardDbContext.EnglishLevels
            .FirstOrDefaultAsync(x => x.Id == request.EnglishLevel, cancellationToken)
            ?? throw new EntityConflictException("Incorrect english level");

        await _identityCrossWriterContract.UpdateUserInternal(
            new CrossWriterUserDto
            {
                Id = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FirstNameRu = request.FirstNameRu,
                LastNameRu = request.LastNameRu,
                PatronymicRu = request.PatronymicRu,
                Email = request.CorporateEmail,
                DepartmentId = request.DepartmentId,
                ResourceGroupId = request.GroupId,
                Position = request.Position,
            }, cancellationToken);

        return await UpdateUserAsync(request, cancellationToken);
    }


    private async Task ValidateHeadOfDepartmentUniqueness(int userId, string newPosition, string? newDepartmentId,
        string? currentPosition, string? currentDepartmentId, CancellationToken cancellationToken)
    {
        var positionChangingToHoD = newPosition.IsHeadOfDepartment() && !currentPosition.IsHeadOfDepartment();
        var departmentChangingForExistingHoD = currentPosition.IsHeadOfDepartment()
            && newDepartmentId != currentDepartmentId;

        if (!positionChangingToHoD && !departmentChangingForExistingHoD)
        {
            return;
        }

        if (newDepartmentId is null)
        {
            return;
        }

        var hodExistsInDepartment = await _teamBoardDbContext.TeamBoardUsers
            .AnyAsync(u => u.UserId != userId
                    && u.DepartmentId == newDepartmentId
                    && u.Position == PortalRoleNames.HeadOfDepartment
                    && u.IsActive,
                cancellationToken);

        if (hodExistsInDepartment)
        {
            var departmentName = await _teamBoardDbContext.TeamboardDepartments
                .Where(d => d.DepartmentId == newDepartmentId)
                .Select(d => d.ShortName)
                .FirstOrDefaultAsync(cancellationToken);

            throw new EntityConflictException($"Head of {departmentName} department already exists");
        }
    }

    private async Task<PersonalDataDto> UpdateUserAsync(UpdatePersonalDataRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == request.UserId)
            .Include(user => user.EnglishLevel)
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.FirstNameRu = request.FirstNameRu;
            user.LastNameRu = request.LastNameRu;
            user.PatronymicRu = request.PatronymicRu;
            user.Email = request.CorporateEmail ?? user.Email;
            user.ResourceGroupId = request.GroupId;
            user.DepartmentId = request.DepartmentId;
            user.PersonalEmail = request.PersonalEmail;
            user.PersonalEmailSecond = request.PersonalEmailSecond;
            user.PersonalPhone = request.PersonalPhone;
            user.Telegram = request.Telegram;
            user.LinkedIn = request.LinkedIn;
            user.EnglishLevelId = request.EnglishLevel;
            user.DateOfBirth = request.DateOfBirth;
            user.Position = request.Position;
            user.MilitaryServiceStatus = request.MilitaryServiceStatus;
            user.IsGraduateJobPlacement = request.IsGraduatedJobPlacement;
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return PersonalDataDto.Of(user!);
    }

    internal sealed record Command(UpdatePersonalDataRequestDto RequestDto)
        : ICommand<PersonalDataDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new UpdatePersonalDataRequestDto.Validator());
        }
    }
}
