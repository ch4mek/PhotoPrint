using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal sealed class UpdateUserAcceptResolverRequest
{
    public InquiriesUserEntity User { get; init; } = null!;
    public string? PrevDepartmentId { get; init; }
    public int? PrevResourceGroupId { get; init; }
    public string? PrevPosition {  get; init; }
    public bool IsPrevActive { get; init; }
}
