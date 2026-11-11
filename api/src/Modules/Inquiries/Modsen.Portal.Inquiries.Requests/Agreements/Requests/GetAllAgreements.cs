using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Loaders;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class GetAllAgreementsRequest : IQueryHandler<GetAllAgreementsRequest.Query, List<UserAgreementCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAgreementsLoader _agreementsLoader;

    public GetAllAgreementsRequest(IInquiriesDbContext inquiriesDbContext, IAgreementsLoader agreementsLoader)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _agreementsLoader = agreementsLoader;
    }

    public async Task<List<UserAgreementCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var agreementIds = await _inquiriesDbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .Select(agreement => agreement.Id)
            .ToArrayAsync(cancellationToken);

        return await _agreementsLoader.LoadAsync(agreementIds, cancellationToken);
    }

    internal sealed record Query: IQuery<List<UserAgreementCoreDto>>;
}
