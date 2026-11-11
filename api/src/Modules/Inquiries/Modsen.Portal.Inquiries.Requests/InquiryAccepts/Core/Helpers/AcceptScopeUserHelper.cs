using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Helpers;

internal class AcceptScopeUserHelper : IEquatable<AcceptScopeUserHelper>
{
    public InquiryAcceptPolicyScopes Scope { get; }
    public int? UserId { get; }
    public int? InquiryTemplateId { get; }

    public AcceptScopeUserHelper(InquiryAcceptPolicyScopes scope, int? userId)
    {
        Scope = scope;
        UserId = scope == InquiryAcceptPolicyScopes.User ? userId : null;
    }

    public AcceptScopeUserHelper(InquiryAcceptPolicyScopes scope, int? userId, int? inquiryTemplateId)
    {
        Scope = scope;
        UserId = scope == InquiryAcceptPolicyScopes.User ? userId : null;
        InquiryTemplateId = inquiryTemplateId;
    }

    public bool Equals(AcceptScopeUserHelper? other)
    {
        if (other == null)
        {
            return false;
        }

        return Scope == other.Scope && UserId == other.UserId && InquiryTemplateId == other.InquiryTemplateId;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as AcceptScopeUserHelper);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Scope, UserId);
    }
}
