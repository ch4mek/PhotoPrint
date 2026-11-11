using Modsen.Portal.Core;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface ISetUserInfoVisibilityResolver
{
    Task ResolveAsync(
        UserInquiryCoreDto inquiry,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);

    Task ResolveAsync(
        IReadOnlyCollection<UserInquiryCoreDto> inquiries,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, bool>> GetVisibilityMapAsync(
        IReadOnlyCollection<int> userIds,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);
}

internal sealed class SetUserInfoVisibilityResolver(ITeamBoardContract teamBoardContract)
    : ISetUserInfoVisibilityResolver
{
    public Task ResolveAsync(
        UserInquiryCoreDto inquiry,
        AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        return ResolveAsync([inquiry], accessOptions, cancellationToken);
    }

    public async Task ResolveAsync(
        IReadOnlyCollection<UserInquiryCoreDto> inquiries,
        AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        if (inquiries.Count == 0)
        {
            return;
        }

        var users = inquiries.SelectMany(EnumerateUsers).ToList();
        var userIds = users.Select(u => u.Id).Distinct().ToArray();

        if (userIds.Length == 0)
        {
            return;
        }

        var visibility = await teamBoardContract.CanUsersBeRetrievedByUser(
            userIds, accessOptions, cancellationToken);

        foreach (var user in users)
        {
            user.CanUserInfoBeRetrievedByCurrentUser = false;

            if (visibility.TryGetValue(user.Id, out var canRetrieve))
            {
                user.CanUserInfoBeRetrievedByCurrentUser = canRetrieve;
            }
        }
    }

    public async Task<IReadOnlyDictionary<int, bool>> GetVisibilityMapAsync(
        IReadOnlyCollection<int> userIds,
        AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<int, bool>();
        }

        var distinct = userIds.Distinct().ToArray();
        return await teamBoardContract.CanUsersBeRetrievedByUser(
            distinct, accessOptions, cancellationToken);
    }

    private static IEnumerable<InquiriesUserDto> EnumerateUsers(UserInquiryCoreDto inquiry)
    {
        yield return inquiry.CreatedByUser;
        yield return inquiry.CreatedForUser;

        if (inquiry.ManagerOfUser is not null)
        {
            yield return inquiry.ManagerOfUser;
        }

        foreach (var accept in inquiry.UserInquiryAccepts)
        {
            if (accept.AcceptUser is not null)
            {
                yield return accept.AcceptUser;
            }

            if (accept.AcceptedByUser is not null)
            {
                yield return accept.AcceptedByUser;
            }

            if (accept.RejectedByUser is not null)
            {
                yield return accept.RejectedByUser;
            }
        }
    }
}
