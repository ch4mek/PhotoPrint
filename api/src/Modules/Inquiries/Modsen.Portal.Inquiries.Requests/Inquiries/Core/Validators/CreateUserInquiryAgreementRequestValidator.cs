using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Validators;

internal interface ICreateUserInquiryAgreementRequestValidator
{
    Task ValidateInquiryAsync(
        CreateUserInquiryAgreementRequest.Command request,
        InquiryTemplateEntity template,
        CancellationToken cancellationToken);

    Task ValidateAgreementAsync(
        CreateUserInquiryAgreementRequest.Command request,
        InquiryTemplateAgreementEntity template,
        InquiryType inquiryType,
        CancellationToken cancellationToken);
}

internal class CreateUserInquiryAgreementRequestValidator : ICreateUserInquiryAgreementRequestValidator
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IIsEnoughAvailableDaysForInquiryBusinessRule _isEnoughAvailableDaysForInquiryBusinessRule;
    private readonly IIdentityContract _identityContract;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CreateUserInquiryAgreementRequestValidator(
        IInquiriesDbContext inquiriesDbContext,
        IClockService clockService,
        ITeamBoardContract teamBoardContract,
        IIsEnoughAvailableDaysForInquiryBusinessRule isEnoughAvailableDaysForInquiryBusinessRule,
        IIdentityContract identityContract,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
        _teamBoardContract = teamBoardContract;
        _isEnoughAvailableDaysForInquiryBusinessRule = isEnoughAvailableDaysForInquiryBusinessRule;
        _identityContract = identityContract;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task ValidateInquiryAsync(
        CreateUserInquiryAgreementRequest.Command request,
        InquiryTemplateEntity template,
        CancellationToken cancellationToken)
    {
        if (!template.IsActive)
        {
            throw new EntityConflictException("You can't create inquiry because it is inactive");
        }

        if (!await HasPermissionToAddInquiryRequest(request.UserId, cancellationToken))
        {
            throw new ActionNotAllowedException("You can't create inquiry for this user");
        }

        if (request.Hours.HasValue && request.Hours.Value > 8)
        {
            throw new InvalidDataAppException("You can't create inquiry with more than 8 hours");
        }

        var userContractDates = await _teamBoardContract.GetUserContractDates(request.UserId, cancellationToken);

        if (template.ProhibitOnProbationPeriod &&
            userContractDates?.HireDate != null &&
            userContractDates.HireDate.Value.AddMonths(3) > _clockService.TodayUtc())
        {
            throw new ActionNotAllowedException("Absence cannot be requested during the probation period");
        }

        _ = await _inquiriesDbContext.UserInquiryTemplates
            .FirstOrDefaultAsync(userInquiryTemplate =>
                userInquiryTemplate.AssignedToUserId == request.UserId &&
                userInquiryTemplate.InquiryTemplateId == template.Id &&
                userInquiryTemplate.DateFrom <= _clockService.TodayUtc() &&
                (userInquiryTemplate.DateTo == null || userInquiryTemplate.DateTo >= _clockService.TodayUtc()),
                cancellationToken)
            ?? throw new InvalidDataAppException("Absence is not assigned to the user");

        if (template.RequireAttachments && (request.Files == null || request.Files.Count == 0))
        {
            throw new InvalidDataAppException("Please attach the required document to proceed");
        }

        if (request.Files?.Count > 10)
        {
            throw new InvalidDataAppException("Max number of attached files is 10");
        }
    }

    public async Task ValidateAgreementAsync(
        CreateUserInquiryAgreementRequest.Command request,
        InquiryTemplateAgreementEntity template,
        InquiryType inquiryType,
        CancellationToken cancellationToken)
    {
        if (!template.InquiryTemplate!.IsActive)
        {
            throw new EntityConflictException("You cannot create agreement because it is inactive");
        }

        if (!await HasPermissionToAddInquiryRequest(request.UserId, cancellationToken))
        {
            throw new ActionNotAllowedException("You can't create inquiry for this user");
        }

        if (request.Hours.HasValue && request.Hours.Value > 8)
        {
            throw new InvalidDataAppException("You can't create inquiry with more than 8 hours");
        }

        var userContractDates = await _teamBoardContract.GetUserContractDates(request.UserId, cancellationToken);

        if (template.InquiryTemplate.ProhibitOnProbationPeriod &&
            userContractDates?.HireDate != null &&
            userContractDates.HireDate.Value.AddMonths(3) > _clockService.TodayUtc())
        {
            throw new ActionNotAllowedException("Absence cannot be requested during the probation period");
        }

        _ = await _inquiriesDbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(userAgreement =>
                userAgreement.UserId == request.UserId &&
                userAgreement.InquiryAgreementTemplateId == template.Id &&
                userAgreement.DateFrom <= _clockService.TodayUtc() &&
                (userAgreement.DateTo == null || userAgreement.DateTo >= _clockService.TodayUtc()),
                cancellationToken)
            ?? throw new InvalidDataAppException("Absence is not assigned to the user");

        if (template.InquiryTemplate.RequireAttachments &&
            (request.Files == null || request.Files.Count == 0))
        {
            throw new InvalidDataAppException("Please attach the required document to proceed");
        }

        if (request.Files?.Count > 10)
        {
            throw new InvalidDataAppException("Max number of attached files is 10");
        }

        if (!await _isEnoughAvailableDaysForInquiryBusinessRule.IsCorrespondsToBusiness(request.InquiryTemplateId,
            request.UserId, request.DateFrom, request.DateTo, cancellationToken))
        {
            throw new EntityConflictException("Request exceeds amount of available days");
        }
    }

    private async Task<bool> HasPermissionToAddInquiryRequest(int userId, CancellationToken cancellationToken)
    {
        if (userId == _authenticatedUserContext.GetRequiredUserId() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.GetRoles().IsCeo() ||
            _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return true;
        }

        var higherManagerIds = await _identityContract.GetHigherManagerIds(userId, cancellationToken);

        return higherManagerIds.Contains(_authenticatedUserContext.GetRequiredUserId());
    }
}
