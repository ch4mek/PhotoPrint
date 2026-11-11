using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface INextApproversToNotifyResolver
{
    Task<List<int>> ResolveAsync(IReadOnlyCollection<UserInquiryAcceptEntity> acceptEntities,
        UserInquiryEntity userInquiry, CancellationToken cancellationToken);
}

internal class NextApproversToNotifyResolver : INextApproversToNotifyResolver
{
    private readonly IGetAbsentApproversResolver _getAbsentApproversResolver;
    private readonly IGetSubstituteResolver _getSubstituteResolver;

    public NextApproversToNotifyResolver(IGetAbsentApproversResolver getAbsentApproversResolver,
        IGetSubstituteResolver getSubstituteResolver)
    {
        _getAbsentApproversResolver = getAbsentApproversResolver;
        _getSubstituteResolver = getSubstituteResolver;
    }

    public async Task<List<int>> ResolveAsync(IReadOnlyCollection<UserInquiryAcceptEntity> acceptEntities,
        UserInquiryEntity userInquiry, CancellationToken cancellationToken)
    {
        var approverIds = acceptEntities.Select(x => x.AcceptUserId).ToList();
        var absentApprovers = await _getAbsentApproversResolver.ResolveAsync(
            approverIds, cancellationToken);

        if (absentApprovers.Count != 0)
        {
            var acceptEntity = acceptEntities.First();
            var substituteId = await _getSubstituteResolver.ResolveAsync(userInquiry.InquiryTemplateId,
                userInquiry.UserId, acceptEntity.AcceptPolicyScope, acceptEntity.AcceptUserId, cancellationToken);

            if (substituteId != null)
            {
                approverIds.Add(substituteId.Value);
            }
        }

        return approverIds;
    }
}
