using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.BusinessRules;

internal interface IArePersonalPhoneAndEmailsUniqueBusinessRule : IBusinessRuleWithFailReason<ArePersonalPhoneAndEmailsUniqueBusinessModel>;

internal class ArePersonalPhoneAndEmailsUniqueBusinessModel
{
    public int? UserId { get; set; }
    public string? PersonalEmail { get; set; }
    public string? PersonalEmailSecond { get; set; }
    public string? PersonalPhone { get; set; }
}

internal class ArePersonalPhoneAndEmailsUniqueBusinessRule : IArePersonalPhoneAndEmailsUniqueBusinessRule
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public ArePersonalPhoneAndEmailsUniqueBusinessRule(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

  public async Task<BusinessRuleResult> IsCorrespondsToBusiness(
        ArePersonalPhoneAndEmailsUniqueBusinessModel? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var phoneCheck = await CheckPhone(model, cancellationToken);
        if (!phoneCheck.IsSuccess)
        {
            return phoneCheck;
        }

        var emailCheck = await CheckEmails(model, cancellationToken);
        if (!emailCheck.IsSuccess)
        {
            return emailCheck;
        }

        return BusinessRuleResult.Success();
    }

    private async Task<BusinessRuleResult> CheckPhone(
        ArePersonalPhoneAndEmailsUniqueBusinessModel model,
        CancellationToken token)
    {
        if (model.PersonalPhone.IsNullOrWhiteSpace())
        {
            return BusinessRuleResult.Success();
        }

        var existingUser = await _teamBoardDbContext.TeamBoardUsers
            .Where(u => u.UserId != model.UserId && u.PersonalPhone == model.PersonalPhone)
            .Select(u => $"{u.UserId}-{u.LastName} {u.FirstName}")
            .FirstOrDefaultAsync(token);

        return existingUser is null
            ? BusinessRuleResult.Success()
            : BusinessRuleResult.Fail($"This phone number is already used by {existingUser}");
    }

    private async Task<BusinessRuleResult> CheckEmails(
        ArePersonalPhoneAndEmailsUniqueBusinessModel model,
        CancellationToken token)
    {
        var emails = ExtractEmails(model);

        if (emails.Count == 0)
        {
            return BusinessRuleResult.Success();
        }

        if (HasDuplicates(emails))
        {
            return BusinessRuleResult.Fail("Personal emails are duplicated");
        }

        var existingEmails = await GetExistingEmails(model.UserId, emails, token);

        var duplicate = FindDuplicateEmail(emails, existingEmails);

        return duplicate is null
            ? BusinessRuleResult.Success()
            : BusinessRuleResult.Fail(string.Format(
                CultureInfo.InvariantCulture,
                PortalMessages.UserWithEmailAlreadyExists,
                duplicate));
    }

    private static List<string> ExtractEmails(ArePersonalPhoneAndEmailsUniqueBusinessModel model)
    {
        return new[]
            {
                model.PersonalEmail,
                model.PersonalEmailSecond
            }
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e!)
            .ToList();
    }

    private static bool HasDuplicates(List<string> emails)
    {
        return emails.Count != emails.Distinct(StringComparer.OrdinalIgnoreCase).Count();
    }

    private async Task<List<(string? Email, string? EmailSecond)>> GetExistingEmails(
        int? userId,
        List<string> emails,
        CancellationToken token)
    {
        return await _teamBoardDbContext.TeamBoardUsers
            .Where(u => u.UserId != userId &&
                        (u.PersonalEmail != null && emails.Any(e => EF.Functions.ILike(e, u.PersonalEmail)) ||
                         u.PersonalEmailSecond != null && emails.Any(e => EF.Functions.ILike(e, u.PersonalEmailSecond))))
            .Select(u => new ValueTuple<string?, string?>(
                u.PersonalEmail,
                u.PersonalEmailSecond))
            .ToListAsync(token);
    }

    private static string? FindDuplicateEmail(
        List<string> emails,
        List<(string? Email, string? EmailSecond)> existing)
    {
        return emails.FirstOrDefault(email =>
            existing.Any(e =>
                (e.Email != null && e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) ||
                (e.EmailSecond != null && e.EmailSecond.Equals(email, StringComparison.OrdinalIgnoreCase))));
    }
}
