using System.Globalization;
using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.ModsenApi.Contract;

namespace Modsen.Portal.Certificate.UseCases;

internal sealed class GetUserCertificatesQueryUseCase : IQueryHandler<GetUserCertificatesQueryUseCase.Query, PageableListOfItems<CertificateDto>>
{
    private readonly IIdentityContract _identityContract;
    private readonly IModsenApiCertificatesClient _modsenApiCertificatesClient;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUserCertificatesQueryUseCase(
        IModsenApiCertificatesClient modsenApiCertificatesClient,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract)
    {
        _modsenApiCertificatesClient = modsenApiCertificatesClient;
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
    }

    public async Task<PageableListOfItems<CertificateDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var queryOptions = request.QueryOptions;
        await ValidateRequest(userId, cancellationToken);

        var certificates = await _modsenApiCertificatesClient.GetUserCertificates(userId.ToString(CultureInfo.InvariantCulture), cancellationToken);
        var csertificatesList = certificates
            .Select(CertificateDto.Of).ToList();

        var certificateDtos = csertificatesList
            .OrderByDescending(c => c.ReceiptDate)
            .AsQueryable();

        var departments = await _identityContract.GetDepartmentModsenApiValues(cancellationToken);

        foreach (var certificate in certificateDtos)
        {
            var department = departments.FirstOrDefault(x => x.ModsenApiValue == certificate.Technology);

            certificate.Technology = department?.Id;
            certificate.Stack = Enum.TryParse(certificate.Stack!, true, out JsStackType jsStackType)
                ? jsStackType.GetDescription()
                : certificate.Stack;
        }

        var totalCount = queryOptions.RebuildWithoutPagination()
            .ApplyTo(certificateDtos).Cast<CertificateDto>().Count();

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<CertificateDto>.Value;
        }

        var result = queryOptions
            .ApplyTo(certificateDtos)
            .Cast<CertificateDto>()
            .ToList();

        return result.ToPageable().WithPaging(request.QueryOptions).WithTotal(totalCount);
    }

    private async Task ValidateRequest(int userId, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return;
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() && _authenticatedUserContext.UserId != userId)
        {
            throw new ActionNotAllowedException("Employee can only access their own certificates");
        }

        await ValidateGroupManagerAccess(userId, cancellationToken);
        await ValidateHeadOfDepartmentAccess(userId, cancellationToken);
    }

    private async Task ValidateGroupManagerAccess(int userId, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.GetRoles().IsGroupManager() || _authenticatedUserContext.UserId == userId)
        {
            return;
        }

        var usersWithManagers = await _identityContract.GetUsersWithManagersAsync([userId], cancellationToken);
        var userManagers = usersWithManagers.FirstOrDefault();

        if (_authenticatedUserContext.ResourceGroupId is null ||
            _authenticatedUserContext.ResourceGroupId != userManagers?.ResourceGroupId)
        {
            throw new ActionNotAllowedException(
                "Group manager can't get certificate of user from another resource group");
        }
    }

    private async Task ValidateHeadOfDepartmentAccess(int userId, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.GetRoles().IsHeadOfDepartment() || _authenticatedUserContext.UserId == userId)
        {
            return;
        }

        var usersWithManagers = await _identityContract.GetUsersWithManagersAsync([userId], cancellationToken);
        var userManagers = usersWithManagers.FirstOrDefault();

        if (_authenticatedUserContext.DepartmentId is null ||
            _authenticatedUserContext.DepartmentId != userManagers?.DepartmentId)
        {
            throw new ActionNotAllowedException("Head can't get certificate of user from another department");
        }
    }


    internal sealed record Query(int UserId, ODataQueryOptions<CertificateDto> QueryOptions)
        : IQuery<PageableListOfItems<CertificateDto>>, ICertificateRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
