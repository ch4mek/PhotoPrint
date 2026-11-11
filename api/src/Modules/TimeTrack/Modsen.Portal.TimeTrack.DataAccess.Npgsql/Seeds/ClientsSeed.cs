using Bogus;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Seeds;

internal class ClientsSeed : ISeedEntitiesProvider<TimeTrackDbContext>
{
    private readonly TimeTrackDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IHexColorGenerator _hexColorGenerator;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly IConfiguration _configuration;
    public ClientsSeed(
        TimeTrackDbContext dbContext,
        IClockService clock,
        IHexColorGenerator hexColorGenerator,
        IHostEnvironmentContext hostEnvironmentContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _clock = clock;
        _hexColorGenerator = hexColorGenerator;
        _hostEnvironmentContext = hostEnvironmentContext;
        _configuration = configuration;
    }

    private int CountOfFakeClients => _hostEnvironmentContext.IsFunctionalTestEnvironment() || _hostEnvironmentContext.IsIntegrationTestEnvironment()
        ? 3
        : 20;

    public void Seed()
    {
        var clients = _dbContext.Set<Client>();
        if (!clients.Any())
        {
            clients.Add(new Client
            {
                Name = _configuration.GetValue<string>("DefaultClientName") ?? "DefaultClient",
                CreatedById = null,
                CreatedAtUtc = _clock.UtcNow,
                Comments = "Modsen is a software development company",
                RegisteredLocation = "Belarus"
            });

            _dbContext.SaveChanges();
        }

        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        if (_hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsFunctionalTestEnvironment() ||
            _hostEnvironmentContext.IsIntegrationTestEnvironment())
        {
            if (clients.Count() >= CountOfFakeClients)
            {
                return;
            }

            var faker = new Faker();

            for (var i = 0; i < CountOfFakeClients; i++)
            {
                var name = faker.Company.CompanyName();

                clients.Add(new Client
                {
                    Name = name,
                    CreatedById = null,
                    CreatedAtUtc = _clock.UtcNow,
                    Comments = faker.Lorem.Sentence(),
                    IsActive = i % 3 != 0,
                    Color = _hexColorGenerator.GenerateColor(),
                    RegisteredLocation = "Belarus"
                });
            }

            _dbContext.SaveChanges();
        }
    }
}
