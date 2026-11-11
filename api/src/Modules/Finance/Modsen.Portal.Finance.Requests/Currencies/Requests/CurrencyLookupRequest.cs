using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Currencies.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Currencies.Requests;

internal sealed class CurrencyLookupRequest : IQueryHandler<CurrencyLookupRequest.Query, List<CurrencyLookupDto>>
{
    private readonly IFinanceDbContext _dbContext;

    public CurrencyLookupRequest(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CurrencyLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var currencies = await _dbContext.Currencies.Select(x => new CurrencyLookupDto
        {
            Id = x.Id,
            CurrencyCode = x.CurrencyCode,
            Name = x.Name,
            CurrencySymbol = x.CurrencySymbol,
        }).ToListAsync(cancellationToken);

        return currencies;
    }

    internal sealed record Query : IQuery<List<CurrencyLookupDto>>;
}
