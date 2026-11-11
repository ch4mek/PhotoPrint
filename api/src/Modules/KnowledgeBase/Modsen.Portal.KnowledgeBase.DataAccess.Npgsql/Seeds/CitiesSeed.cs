using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Seeds;

internal class CitiesSeed : ISeedEntitiesProvider<KnowledgeBaseDbContext>
{
    private readonly KnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public CitiesSeed(KnowledgeBaseDbContext knowledgeBaseDbContext,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if ((_hostEnvironmentContext.IsDevelopmentOrQaEnvironment() || _hostEnvironmentContext.IsIntegrationTestEnvironment()) &&
            !_knowledgeBaseDbContext.Cities.Any())
        {
            _knowledgeBaseDbContext.Cities.AddRange(GetCities()
                .Select(city => new CityEntity
                {
                    Name = city
                }));
            _knowledgeBaseDbContext.SaveChanges();
        }
    }

    private static IEnumerable<string> GetCities()
    {
        yield return "Minsk";
        yield return "Vitebsk";
        yield return "Brest";
        yield return "Gomel";
        yield return "Grodno";
        yield return "Mogilev";
    }
}
