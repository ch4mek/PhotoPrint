using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;

internal interface IAcceptPoliciesValidator
{
    Task Validate(List<InquiryTemplateAcceptPolicyDto> policies, CancellationToken cancellationToken);
}

internal class AcceptPoliciesValidator : IAcceptPoliciesValidator
{
    private readonly IUserLoader _userLoader;

    public AcceptPoliciesValidator(IUserLoader userLoader)
    {
        _userLoader = userLoader;
    }

    public async Task Validate(List<InquiryTemplateAcceptPolicyDto> policies, CancellationToken cancellationToken)
    {
        var userIds = policies
            .Where(p => p.UserId != null)
            .Select(p => p.UserId!.Value)
            .Distinct()
            .ToArray();

        var users = await _userLoader.GetUsersByIdsAsync(userIds, cancellationToken);

        var missing = userIds.Except(users.Select(user => user.Id)).ToList();
        if (missing.Count != 0)
        {
            throw new EntityNotFoundException(
                $"Users [{string.Join(", ", missing)}] couldn't be found for accept policies");
        }
    }
}
