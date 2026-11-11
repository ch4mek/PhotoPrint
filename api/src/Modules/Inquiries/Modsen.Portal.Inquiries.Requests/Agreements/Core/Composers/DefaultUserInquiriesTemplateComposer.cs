using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;

// Used for PeopleForce sync, should be reworked

[Obsolete("Used for PeopleForce sync, should be reworked")]
internal interface IDefaultUserInquiriesTemplateComposer
{
    Task<List<UserInquiryTemplateEntity>> Compose(int userId, DateOnly hireDate, CancellationToken cancellationToken);
    Task<List<UserInquiryTemplateEntity>> Compose(int userId, CancellationToken cancellationToken);
}

[Obsolete("Used for PeopleForce sync, should be reworked")]
internal class DefaultUserInquiriesTemplateComposer : IDefaultUserInquiriesTemplateComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly ITeamBoardContract _teamBoardContract;

    public DefaultUserInquiriesTemplateComposer(
        IInquiriesDbContext inquiriesDbContext,
        ITeamBoardContract teamBoardContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _teamBoardContract = teamBoardContract;
    }

    // tests for this method will be added after its fixes
    public async Task<List<UserInquiryTemplateEntity>> Compose(int userId, DateOnly hireDate, CancellationToken cancellationToken)
    {
        var userTemplates = new List<UserInquiryTemplateEntity>();

        var defaultTemplates = await _inquiriesDbContext.InquiryTemplates
            .Where(x => x.IsDefault)
            .ToListAsync(cancellationToken);

        var existedTemplates = await _inquiriesDbContext.UserInquiryTemplates
            .Include(x => x.InquiryTemplate)
            .Where(x => x.AssignedToUserId == userId && x.InquiryTemplate!.IsDefault)
            .ToListAsync(cancellationToken);

        var existingDefaultAgreements = await _inquiriesDbContext.InquiryTemplateAgreements
            .Where(x => !x.IsDeleted)
            .Where(x => x.IsDefault && defaultTemplates.Select(it => it.Id).Contains(x.InquiryTemplateId))
            .ToListAsync(cancellationToken);

        foreach (var template in defaultTemplates)
        {
            if (existedTemplates.All(x => x.InquiryTemplate!.InquiryType != template.InquiryType) &&
                existingDefaultAgreements.All(x => x.InquiryTemplateId != template.Id))
            {
                var usersInquiryTemplate = new UserInquiryTemplateEntity
                {
                    DateFrom = hireDate,
                    DateTo = null,
                    AssignedToUserId = userId,
                    InquiryTemplateId = template.Id,
                };

                userTemplates.AddRange(usersInquiryTemplate);
            }
        }

        return userTemplates;
    }

    public async Task<List<UserInquiryTemplateEntity>> Compose(int userId, CancellationToken cancellationToken)
    {
        var userTemplates = new List<UserInquiryTemplateEntity>();

        var defaultTemplates = await _inquiriesDbContext.InquiryTemplates
            .Where(x =>
                x.IsDefault &&
                !x.InquiryTemplateAgreements.Any(x => !x.IsDeleted))
            .ToListAsync(cancellationToken);

        var userContractDates = await _teamBoardContract.GetUserContractDates(userId, cancellationToken);

        if (userContractDates?.HireDate is null)
        {
            throw new EntityConflictException($"User {userId} doesn't have a contract with hire date");
        }

        foreach (var template in defaultTemplates)
        {
            var usersInquiryTemplate = new UserInquiryTemplateEntity
            {
                DateFrom = userContractDates.HireDate.Value,
                DateTo = userContractDates.FireDate,
                AssignedToUserId = userId,
                InquiryTemplateId = template.Id,
            };

            userTemplates.AddRange(usersInquiryTemplate);
        }

        return userTemplates;
    }
}
